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

	public PaymentWorkflowService(
		IVnPayService vnPayService,
		IPaymentService paymentService,
		IOrderService orderService,
		ICartService cartService,
		IProductService productService,
		IUserVoucherService userVoucherService)
	{
		_vnPayService = vnPayService;
		_paymentService = paymentService;
		_orderService = orderService;
		_cartService = cartService;
		_productService = productService;
		_userVoucherService = userVoucherService;
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
}


