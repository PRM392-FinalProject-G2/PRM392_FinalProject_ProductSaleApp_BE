using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class PaymentWorkflowService : IPaymentWorkflowService
{
	private readonly IVnPayService _vnPayService;
	private readonly IPaymentService _paymentService;
	private readonly IOrderService _orderService;
	private readonly ICartService _cartService;
	private readonly IProductService _productService;
	private readonly IUserVoucherService _userVoucherService;
	private readonly IVoucherService _voucherService;
	private readonly IProductVoucherService _productVoucherService;
	private readonly ProductSaleApp.Repository.UnitOfWork.IUnitOfWork _unitOfWork;

	public PaymentWorkflowService(
		IVnPayService vnPayService,
		IPaymentService paymentService,
		IOrderService orderService,
		ICartService cartService,
		IProductService productService,
		IUserVoucherService userVoucherService,
		IVoucherService voucherService,
		IProductVoucherService productVoucherService,
		ProductSaleApp.Repository.UnitOfWork.IUnitOfWork unitOfWork)
	{
		_vnPayService = vnPayService;
		_paymentService = paymentService;
		_orderService = orderService;
		_cartService = cartService;
		_productService = productService;
		_userVoucherService = userVoucherService;
		_voucherService = voucherService;
		_productVoucherService = productVoucherService;
		_unitOfWork = unitOfWork;
	}

	public async Task<VnPayCallbackResult> ProcessVnPayCallbackAsync(IQueryCollection query)
	{
		var ok = _vnPayService.ValidateCallback(query, out var status, out var txnRef, out var amount, out var message);
		if (!ok)
		{
			return new VnPayCallbackResult { Success = false, OrderId = 0, Amount = 0m, Message = "Invalid signature" };
		}

		if (!int.TryParse(txnRef, out var paymentId))
		{
			return new VnPayCallbackResult { Success = false, OrderId = 0, Amount = 0m, Message = "Invalid txn ref" };
		}

		var payment = await _paymentService.GetByIdAsync(paymentId);
		if (payment == null)
		{
			return new VnPayCallbackResult { Success = false, OrderId = 0, Amount = 0m, Message = "Payment not found" };
		}

		var orderId = payment.OrderId ?? 0;
		var order = orderId > 0 ? await _orderService.GetByIdAsync(orderId) : null;

		if (status == "00")
		{
			payment.PaymentStatus = "Paid";
			if (order != null) order.OrderStatus = "Delivering";

			if (order?.CartId.HasValue == true)
			{
				// Use AsNoTracking to avoid tracking conflicts later
				var cart = await _cartService.GetByIdAsync(order.CartId.Value);
				if (cart?.CartItems?.Any() == true)
				{
					var productIds = cart.CartItems
						.Where(ci => ci.ProductId.HasValue)
						.Select(ci => ci.ProductId.Value)
						.ToList();

					if (productIds.Any())
					{
						await _productService.IncrementPopularityAsync(productIds, 1);
					}
				}
			}

			if (order?.UserId.HasValue == true)
			{
				var uv = await _userVoucherService.GetByUserIdAndOrderIdAsync(order.UserId.Value, order.OrderId);
				if (uv != null && !uv.IsUsed)
				{
					uv.IsUsed = true;
					uv.UsedAt = DateTime.Now;
					await _userVoucherService.UpdateAsync(uv.UserVoucherId, uv);
				}
			}

			// Handle cart management after successful payment
			if (order != null)
			{
				await HandleSuccessfulPaymentCartManagementAsync(order.OrderId);
			}
		}
		else
		{
			payment.PaymentStatus = "Failed";
			if (order != null) order.OrderStatus = "Failed";

			if (order?.OrderId > 0)
			{
				var userVouchers = await _userVoucherService.GetPagedFilteredAsync(new UserVoucherBM
				{
					OrderId = order.OrderId,
					IsUsed = false
				}, 1, 10);
				var uv = userVouchers.Items.FirstOrDefault();
				if (uv != null)
				{
					uv.OrderId = null;
					await _userVoucherService.UpdateAsync(uv.UserVoucherId, uv);
				}
			}
		}

		await _paymentService.UpdateAsync(payment.PaymentId, payment);
		if (order != null)
		{
			await _orderService.UpdateAsync(order.OrderId, order);
		}

		return new VnPayCallbackResult
		{
			Success = status == "00",
			OrderId = orderId,
			Amount = amount,
			Message = message
		};
	}

	public async Task<CreateOrderPaymentResult> CreateOrderAndPaymentAsync(OrderBM orderRequest, int? voucherId, string clientIp)
	{
		try
		{
			// Validate cart first
			if (!orderRequest.CartId.HasValue)
			{
				return new CreateOrderPaymentResult { Success = false, Message = "CartId is required" };
			}

			var cart = await _cartService.GetByIdAsync(orderRequest.CartId.Value);
			if (cart == null)
			{
				return new CreateOrderPaymentResult { Success = false, Message = "Cart not found" };
			}

			// Calculate amount from cart
			decimal amount = 0m;
			if (cart.CartItems != null && cart.CartItems.Count > 0)
			{
				amount = cart.CartItems.Sum(ci => ci.Price * ci.Quantity);
			}
			else
			{
				amount = cart.TotalPrice;
			}

			// Validate and calculate voucher discount BEFORE creating order
			decimal voucherDiscount = 0m;
			if (voucherId.HasValue)
			{
				Console.WriteLine($"[Voucher] Start validate VoucherId={voucherId.Value} for UserId={orderRequest.UserId}");
				
				var voucher = await _voucherService.GetByIdAsync(voucherId.Value);
				if (voucher == null) 
					return new CreateOrderPaymentResult { Success = false, Message = "Voucher không tồn tại" };
				if (!voucher.IsActive) 
					return new CreateOrderPaymentResult { Success = false, Message = "Voucher không còn hoạt động" };
				
				var now = DateTime.Now;
				if (now < voucher.StartDate || now > voucher.EndDate) 
					return new CreateOrderPaymentResult { Success = false, Message = "Voucher không trong thời gian hiệu lực" };
				
				Console.WriteLine($"[Voucher] Voucher active & valid period. Start={voucher.StartDate:O}, End={voucher.EndDate:O}, Percent={voucher.DiscountPercent}, Amount={voucher.DiscountAmount}");

				// Check if user owns the voucher
				var userVouchers = await _userVoucherService.GetPagedFilteredAsync(new UserVoucherBM 
				{ 
					UserId = orderRequest.UserId.Value, 
					VoucherId = voucherId.Value 
				}, 1, 10);
				var userVoucher = userVouchers.Items.FirstOrDefault();
				if (userVoucher == null) 
					return new CreateOrderPaymentResult { Success = false, Message = "Bạn không sở hữu voucher này" };
				if (userVoucher.IsUsed) 
					return new CreateOrderPaymentResult { Success = false, Message = "Voucher đã được sử dụng" };
				
				Console.WriteLine($"[Voucher] UserVoucher found. IsUsed={userVoucher.IsUsed}, UserVoucherId={userVoucher.UserVoucherId}");

				// Check product-specific vouchers
				var productVouchers = await _productVoucherService.GetPagedFilteredAsync(new ProductVoucherBM { VoucherId = voucherId.Value }, 1, 1000);
				if (productVouchers.Items.Any())
				{
					Console.WriteLine($"[Voucher] Product-specific voucher. ProductVoucher count={productVouchers.Items.Count}");
					var cartProductIds = cart.CartItems
						.Select(ci => ci.ProductId).Where(id => id.HasValue).Select(id => id.Value);
					var voucherProductIds = productVouchers.Items.Select(pv => pv.ProductId);
					var hasValidProduct = cartProductIds.Any(id => voucherProductIds.Contains(id));
					if (!hasValidProduct) 
						return new CreateOrderPaymentResult { Success = false, Message = "Voucher không áp dụng cho sản phẩm trong giỏ hàng" };
					
					Console.WriteLine($"[Voucher] Matched products in cart: [{string.Join(", ", cartProductIds.Where(id => voucherProductIds.Contains(id)).Select(id => id.ToString()))}]");

					// Calculate discount for product-specific voucher
					if (voucher.DiscountPercent.HasValue)
					{
						var applicableItems = cart.CartItems.Where(ci => ci.ProductId.HasValue && voucherProductIds.Contains(ci.ProductId.Value));
						foreach (var item in applicableItems)
						{
							var lineTotal = item.Price * item.Quantity;
							Console.WriteLine($"[Voucher] Item productId={item.ProductId}, price={item.Price}, qty={item.Quantity}, lineTotal={lineTotal}");
						}
						var applicableAmount = applicableItems.Sum(ci => ci.Price * ci.Quantity);
						Console.WriteLine($"[Voucher] applicableAmount={applicableAmount}, percent={voucher.DiscountPercent.Value}");
						voucherDiscount = applicableAmount * (voucher.DiscountPercent.Value / 100);
						Console.WriteLine($"[Voucher] voucherDiscount (product-specific percent) = {voucherDiscount}");
					}
					else if (voucher.DiscountAmount.HasValue)
					{
						voucherDiscount = voucher.DiscountAmount.Value;
						Console.WriteLine($"[Voucher] voucherDiscount (product-specific fixed amount) = {voucherDiscount}");
					}
				}
				else
				{
					Console.WriteLine($"[Voucher] Global voucher (no product restriction)");
					var totalCartAmount = cart.CartItems.Sum(ci => ci.Price * ci.Quantity);
					Console.WriteLine($"[Voucher] totalCartAmount={totalCartAmount}");
					
					if (voucher.DiscountPercent.HasValue)
					{
						voucherDiscount = totalCartAmount * (voucher.DiscountPercent.Value / 100);
						Console.WriteLine($"[Voucher] voucherDiscount (global percent) = {voucherDiscount}");
					}
					else if (voucher.DiscountAmount.HasValue)
					{
						voucherDiscount = voucher.DiscountAmount.Value;
						Console.WriteLine($"[Voucher] voucherDiscount (global fixed amount) = {voucherDiscount}");
					}
				}
			}

			var finalAmount = amount - voucherDiscount;
			Console.WriteLine($"[Voucher] amount(before)={amount}, voucherDiscount={voucherDiscount}, finalAmount={finalAmount}");

			// NOW create order after all validations pass
			orderRequest.OrderStatus = "Pending";
			var createdOrder = await _orderService.CreateAsync(orderRequest);
			var order = await _orderService.GetByIdAsync(createdOrder.OrderId);

			// Create payment
			var payment = await _paymentService.CreateAsync(new PaymentBM
			{
				OrderId = order.OrderId,
				Amount = finalAmount,
				PaymentStatus = "Pending",
				PaymentDate = DateTime.Now
			});

			// Link voucher with order but don't mark as used yet
			if (voucherId.HasValue)
			{
				var linkUserVouchers = await _userVoucherService.GetPagedFilteredAsync(new UserVoucherBM 
				{ 
					UserId = orderRequest.UserId.Value, 
					VoucherId = voucherId.Value, 
					IsUsed = false 
				}, 1, 10);
				var uv = linkUserVouchers.Items.FirstOrDefault();
				if (uv != null)
				{
					uv.OrderId = order.OrderId;
					await _userVoucherService.UpdateAsync(uv.UserVoucherId, uv);
				}
			}

			// Create VnPay payment URL
			var url = _vnPayService.CreatePaymentUrl(payment.PaymentId, order.OrderId, finalAmount, clientIp);

			return new CreateOrderPaymentResult
			{
				Success = true,
				OrderId = order.OrderId,
				PaymentId = payment.PaymentId,
				OriginalAmount = amount,
				VoucherDiscount = voucherDiscount,
				FinalAmount = finalAmount,
				PaymentUrl = url
			};
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[Error] CreateOrderAndPaymentAsync: {ex.Message}");
			return new CreateOrderPaymentResult { Success = false, Message = ex.Message };
		}
	}

	public async Task<bool> HandleSuccessfulPaymentCartManagementAsync(int orderId)
	{
		try
		{
			// Get order to find the cart
			var order = await _orderService.GetByIdAsync(orderId);
			if (order == null || !order.CartId.HasValue)
			{
				Console.WriteLine($"[CartManagement] Order {orderId} not found or has no cart");
				return false;
			}

			var cartId = order.CartId.Value;
			var userId = order.UserId;

			// Create new Active cart for the user first
			var newCart = new CartBM
			{
				UserId = userId,
				TotalPrice = 0,
				Status = "Active"
			};

			var createdCart = await _cartService.CreateAsync(newCart);
			Console.WriteLine($"[CartManagement] Created new cart {createdCart.CartId} with status Active for user {userId}");

			// Update the old cart status to Completed using dedicated method to avoid tracking conflicts
			var updateResult = await _cartService.UpdateCartStatusAsync(cartId, "Completed");
			if (updateResult)
			{
				Console.WriteLine($"[CartManagement] Updated cart {cartId} status to Completed");
			}
			else
			{
				Console.WriteLine($"[CartManagement] Failed to update cart {cartId} status");
			}

			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[Error] HandleSuccessfulPaymentCartManagementAsync: {ex.Message}");
			return false;
		}
	}
}




