using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;
    private readonly IMapper _mapper;
    private readonly IVnPayService _vnPayService;
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private readonly IConfiguration _configuration;

    public PaymentsController(IPaymentService service, IMapper mapper, IVnPayService vnPayService, IOrderService orderService, ICartService cartService, IConfiguration configuration, IProductService productService)
    {
        _service = service;
        _mapper = mapper;
        _vnPayService = vnPayService;
        _orderService = orderService;
        _cartService = cartService;
        _configuration = configuration;
        _productService = productService;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<PagedResponse<PaymentResponse>>> GetFilter([FromQuery] PaymentGetRequest request)
    {
        var filter = _mapper.Map<PaymentBM>(request);
        var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
        return Ok(_mapper.Map<PagedResponse<PaymentResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PaymentResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<PaymentResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> Create(PaymentRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<PaymentBM>(request));
        var response = _mapper.Map<PaymentResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.PaymentId }, response);
    }

    [HttpGet("vnpay/callback")]
    public async Task<ActionResult<object>> VnPayCallback()
    {
        var ok = _vnPayService.ValidateCallback(Request.Query, out var status, out var txnRef, out var amount, out var message);
        if (!ok)
        {
            return BadRequest(new { success = false, message = "Invalid signature" });
        }

        if (!int.TryParse(txnRef, out var paymentId))
        {
            return BadRequest(new { success = false, message = "Invalid txn ref" });
        }

        var payment = await _service.GetByIdAsync(paymentId);
        if (payment == null)
        {
            return NotFound(new { success = false, message = "Payment not found" });
        }
        var orderId = payment.OrderId ?? 0;
        var order = orderId > 0 ? await _orderService.GetByIdAsync(orderId) : null;

        if (status == "00")
        {
            payment.PaymentStatus = "Success";
            order.OrderStatus = "Paid";
            // +1 popularity cho các sản phẩm trong cart của order
            if (order?.CartId != null)
            {
                var cart = await _cartService.GetByIdAsync(order.CartId.Value);
                if (cart?.CartItems != null && cart.CartItems.Count > 0)
                {
                    var productIds = cart.CartItems.Select(ci => ci.ProductId).Where(id => id.HasValue).Select(id => id.Value);
                    await _productService.IncrementPopularityAsync(productIds, 1);
                }
            }
        }
        else
        {
            payment.PaymentStatus = "Failed";
            order.OrderStatus = "Failed";
        }

        await _service.UpdateAsync(payment.PaymentId, payment);
        if (order != null)
        {
            await _orderService.UpdateAsync(order.OrderId, order);
        }

        var success = status == "00";
        var feUrl = _configuration["VnPay:FrontendReturnUrl"];
        if (!string.IsNullOrWhiteSpace(feUrl))
        {
            var redirectUrl = feUrl + $"?orderId={orderId}&success={success.ToString().ToLower()}&amount={amount}";
            return Redirect(redirectUrl);
        }
        return Ok(new { success, orderId, amount, message });
    }

    [HttpPost("vnpay/create-order")]
    public async Task<ActionResult<object>> CreateOrderAndVnPayUrl(OrderRequest request)
    {
        var orderModel = _mapper.Map<OrderBM>(request);
        orderModel.UserId = request.UserId; // đảm bảo gán đúng UserId
        orderModel.OrderStatus = "Pending";
        var createdOrder = await _orderService.CreateAsync(orderModel);
        var order = await _orderService.GetByIdAsync(createdOrder.OrderId);

        decimal amount = 0m;
        if (order.CartId.HasValue)
        {
            var cart = await _cartService.GetByIdAsync(order.CartId.Value);
            if (cart == null) return BadRequest("Cart not found");
            if (cart.CartItems != null && cart.CartItems.Count > 0)
            {
                amount = cart.CartItems.Sum(ci => ci.Price * ci.Quantity);
            }
            else
            {
                amount = cart.TotalPrice;
            }
        }
        else
        {
            return BadRequest("CartId is required to compute amount");
        }

        var payment = await _service.CreateAsync(new PaymentBM
        {
            OrderId = order.OrderId,
            Amount = amount,
            PaymentStatus = "Pending",
            PaymentDate = System.DateTime.Now
        });

        order.OrderStatus = "Pending";
        await _orderService.UpdateAsync(order.OrderId, order);

        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var url = _vnPayService.CreatePaymentUrl(payment.PaymentId, order.OrderId, amount, clientIp);
        return Ok(new { orderId = order.OrderId, paymentId = payment.PaymentId, amount, paymentUrl = url });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PaymentResponse>> Update(int id, PaymentRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<PaymentBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<PaymentResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}


