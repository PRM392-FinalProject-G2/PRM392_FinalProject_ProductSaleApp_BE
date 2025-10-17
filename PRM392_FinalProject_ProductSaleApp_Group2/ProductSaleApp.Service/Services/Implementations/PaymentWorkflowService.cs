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
    // Helper: delete payment by id using payment service

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

	private async Task _serviceDeletePaymentIfPossible(int paymentId)
	{
		try
		{
			await _paymentService.DeleteAsync(paymentId);
		}
		catch { /* ignore delete failures */ }
	}

	public async Task<AmountCalculationResult> CalculateOrderAmountAsync(int cartId, int? userId, int? voucherId)
	{
		try
		{
			var cart = await _cartService.GetByIdAsync(cartId);
			if (cart == null)
			{
				return new AmountCalculationResult { Success = false, Message = "Cart not found" };
			}

			decimal amount = 0m;
			if (cart.CartItems != null && cart.CartItems.Count > 0)
			{
				amount = cart.CartItems.Sum(ci => ci.Price * ci.Quantity);
			}
			else
			{
				amount = cart.TotalPrice;
			}

			decimal voucherDiscount = 0m;
			if (voucherId.HasValue)
			{
				if (!userId.HasValue)
					return new AmountCalculationResult { Success = false, Message = "UserId is required when applying voucher" };

				var voucher = await _voucherService.GetByIdAsync(voucherId.Value);
				if (voucher == null)
					return new AmountCalculationResult { Success = false, Message = "Voucher không tồn tại" };
				if (!voucher.IsActive)
					return new AmountCalculationResult { Success = false, Message = "Voucher không còn hoạt động" };
				var now = DateTime.Now;
				if (now < voucher.StartDate || now > voucher.EndDate)
					return new AmountCalculationResult { Success = false, Message = "Voucher không trong thời gian hiệu lực" };

				var userVouchers = await _userVoucherService.GetPagedFilteredAsync(new UserVoucherBM
				{
					UserId = userId.Value,
					VoucherId = voucherId.Value
				}, 1, 10);
				var userVoucher = userVouchers.Items.FirstOrDefault();
				if (userVoucher == null)
					return new AmountCalculationResult { Success = false, Message = "Bạn không sở hữu voucher này" };
				if (userVoucher.IsUsed)
					return new AmountCalculationResult { Success = false, Message = "Voucher đã được sử dụng" };

				var productVouchers = await _productVoucherService.GetPagedFilteredAsync(new ProductVoucherBM { VoucherId = voucherId.Value }, 1, 1000);
				if (productVouchers.Items.Any())
				{
					var cartProductIds = cart.CartItems.Select(ci => ci.ProductId).Where(id => id.HasValue).Select(id => id.Value);
					var voucherProductIds = productVouchers.Items.Select(pv => pv.ProductId);
					var hasValidProduct = cartProductIds.Any(id => voucherProductIds.Contains(id));
					if (!hasValidProduct)
						return new AmountCalculationResult { Success = false, Message = "Voucher không áp dụng cho sản phẩm trong giỏ hàng" };

					if (voucher.DiscountPercent.HasValue)
					{
						var applicableItems = cart.CartItems.Where(ci => ci.ProductId.HasValue && voucherProductIds.Contains(ci.ProductId.Value));
						var applicableAmount = applicableItems.Sum(ci => ci.Price * ci.Quantity);
						voucherDiscount = applicableAmount * (voucher.DiscountPercent.Value / 100);
					}
					else if (voucher.DiscountAmount.HasValue)
					{
						voucherDiscount = voucher.DiscountAmount.Value;
					}
				}
				else
				{
					var totalCartAmount = cart.CartItems.Sum(ci => ci.Price * ci.Quantity);
					if (voucher.DiscountPercent.HasValue)
						voucherDiscount = totalCartAmount * (voucher.DiscountPercent.Value / 100);
					else if (voucher.DiscountAmount.HasValue)
						voucherDiscount = voucher.DiscountAmount.Value;
				}
			}

			var finalAmount = amount - voucherDiscount;
			return new AmountCalculationResult
			{
				Success = true,
				OriginalAmount = amount,
				VoucherDiscount = voucherDiscount,
				FinalAmount = finalAmount
			};
		}
		catch (Exception ex)
		{
			return new AmountCalculationResult { Success = false, Message = ex.Message };
		}
	}

	public async Task<CreateOrderPaymentResult> CreatePaymentWithFinalAmountAsync(OrderBM orderRequest, int? voucherId, decimal finalAmount, string clientIp)
	{
		try
		{
			if (!orderRequest.CartId.HasValue)
				return new CreateOrderPaymentResult { Success = false, Message = "CartId is required" };

			// Step now: DO NOT create order yet. Create a pending payment first to obtain paymentId (vnp_TxnRef)
			var payment = await _paymentService.CreateAsync(new PaymentBM
			{
				OrderId = null,
				Amount = finalAmount,
				PaymentStatus = "Pending",
				PaymentDate = DateTime.Now
			});

			// Embed order metadata so callback can create order if success
			var meta = new
			{
				cartId = orderRequest.CartId,
				userId = orderRequest.UserId,
				voucherId = voucherId,
				finalAmount = finalAmount,
				paymentMethod = orderRequest.PaymentMethod,
				billingAddress = orderRequest.BillingAddress
			};
			var orderInfo = System.Text.Json.JsonSerializer.Serialize(meta);

			var url = _vnPayService.CreatePaymentUrl(payment.PaymentId, 0, finalAmount, clientIp, orderInfo);
			return new CreateOrderPaymentResult
			{
				Success = true,
				OrderId = 0,
				PaymentId = payment.PaymentId,
				OriginalAmount = finalAmount,
				VoucherDiscount = 0,
				FinalAmount = finalAmount,
				PaymentUrl = url
			};
		}
		catch (Exception ex)
		{
			return new CreateOrderPaymentResult { Success = false, Message = ex.Message };
		}
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

			// If order hasn't been created yet (new flow), create it now from vnp_OrderInfo
			if (order == null)
			{
				var orderInfoJson = query["vnp_OrderInfo"].ToString();
				try
				{
					var meta = System.Text.Json.JsonSerializer.Deserialize<OrderInfoMeta>(orderInfoJson);
					var newOrder = new OrderBM
					{
						CartId = meta?.cartId,
						UserId = meta?.userId,
						OrderStatus = "Pending",
						OrderDate = DateTime.Now,
						PaymentMethod = meta?.paymentMethod,
						BillingAddress = meta?.billingAddress
					};
					var created = await _orderService.CreateAsync(newOrder);
					order = await _orderService.GetByIdAsync(created.OrderId);
					orderId = order.OrderId;
					payment.OrderId = orderId;
					payment.Amount = meta?.finalAmount ?? amount;

					// Link voucher for later marking used
					if (meta?.voucherId != null && meta?.userId != null)
					{
						var linkUserVouchers = await _userVoucherService.GetPagedFilteredAsync(new UserVoucherBM
						{
							UserId = meta.userId.Value,
							VoucherId = meta.voucherId.Value,
							IsUsed = false
						}, 1, 10);
						var uv = linkUserVouchers.Items.FirstOrDefault();
						if (uv != null)
						{
							uv.OrderId = orderId;
							await _userVoucherService.UpdateAsync(uv.UserVoucherId, uv);
						}
					}
				}
				catch { /* ignore malformed metadata */ }
			}

			if (order != null) order.OrderStatus = "Pending";

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
				// Use finalAmount from metadata if available, otherwise use payment.Amount
				var finalAmount = payment.Amount;
				if (orderId > 0 && order.OrderId == orderId)
				{
					// This is a new order created from metadata, use the finalAmount from metadata
					var orderInfoJson = query["vnp_OrderInfo"].ToString();
					try
					{
						var meta = System.Text.Json.JsonSerializer.Deserialize<OrderInfoMeta>(orderInfoJson);
						if (meta?.finalAmount.HasValue == true)
						{
							finalAmount = meta.finalAmount.Value;
						}
					}
					catch { /* ignore malformed metadata */ }
				}
				
				await HandleSuccessfulPaymentCartManagementAsync(order.OrderId, finalAmount);
			}
		}
		else
		{
			// New flow: on fail, do not create order; if there's a pending payment without order, delete it
			payment.PaymentStatus = "Failed";
			if (payment.OrderId == null || payment.OrderId == 0)
			{
				await _serviceDeletePaymentIfPossible(payment.PaymentId);
			}
			else
			{
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
			orderRequest.OrderDate = DateTime.Now;
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

	public async Task<bool> HandleSuccessfulPaymentCartManagementAsync(int orderId, decimal finalAmount)
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

			// Update the old cart status to Completed and set its TotalPrice to final amount
			var updateResult = await _cartService.UpdateCartStatusAsync(cartId, "Completed");
			if (updateResult)
			{
				// Update TotalPrice using the same approach as UpdateCartStatusAsync
				var cart = await _unitOfWork.CartRepository.GetByIdAsync(cartId, trackChanges: true);
				if (cart != null)
				{
					cart.Totalprice = finalAmount;
					_unitOfWork.CartRepository.Update(cart);
					await _unitOfWork.SaveChangesAsync();
					Console.WriteLine($"[CartManagement] Updated cart {cartId} status to Completed with totalPrice={finalAmount}");
				}
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

internal class OrderInfoMeta
{
    public int? cartId { get; set; }
    public int? userId { get; set; }
    public int? voucherId { get; set; }
    public decimal? finalAmount { get; set; }
    public string paymentMethod { get; set; }
    public string billingAddress { get; set; }
}




