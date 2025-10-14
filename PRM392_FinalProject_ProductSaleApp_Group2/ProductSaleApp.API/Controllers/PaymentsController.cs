using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;
    private readonly IMapper _mapper;
    private readonly IVnPayService _vnPayService;
    private readonly IPaymentWorkflowService _paymentWorkflowService;
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private readonly IConfiguration _configuration;
    private readonly IVoucherService _voucherService;
    private readonly IUserVoucherService _userVoucherService;
    private readonly IProductVoucherService _productVoucherService;

    public PaymentsController(IPaymentService service, IMapper mapper, IVnPayService vnPayService, IOrderService orderService, ICartService cartService, IConfiguration configuration, IProductService productService, IVoucherService voucherService, IUserVoucherService userVoucherService, IProductVoucherService productVoucherService, IPaymentWorkflowService paymentWorkflowService)
    {
        _service = service;
        _mapper = mapper;
        _vnPayService = vnPayService;
        _orderService = orderService;
        _cartService = cartService;
        _configuration = configuration;
        _productService = productService;
        _voucherService = voucherService;
        _userVoucherService = userVoucherService;
        _productVoucherService = productVoucherService;
        _paymentWorkflowService = paymentWorkflowService;
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
        var result = await _paymentWorkflowService.ProcessVnPayCallbackAsync(Request.Query);
        if (!result.Success && result.Message == "Invalid signature") return BadRequest(new { success = false, message = result.Message });
        if (!result.Success && result.Message is "Invalid txn ref" or "Payment not found") return NotFound(new { success = false, message = result.Message });

        var feUrl = _configuration["VnPay:FrontendReturnUrl"];
        if (!string.IsNullOrWhiteSpace(feUrl))
        {
            var redirectUrl = feUrl + $"?orderId={result.OrderId}&success={result.Success.ToString().ToLower()}&amount={result.Amount}";
            return Redirect(redirectUrl);
        }
        return Ok(new { success = result.Success, orderId = result.OrderId, amount = result.Amount, message = result.Message });
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

        // Validate voucher (inline) và tính finalAmount
        decimal voucherDiscount = 0m;
        if (request.VoucherId.HasValue)
        {
			Console.WriteLine($"[Voucher] Start validate VoucherId={request.VoucherId.Value} for OrderId={order.OrderId}, UserId={request.UserId}");
            var voucher = await _voucherService.GetByIdAsync(request.VoucherId.Value);
            if (voucher == null) return BadRequest(new { message = "Voucher không tồn tại" });
            if (!voucher.IsActive) return BadRequest(new { message = "Voucher không còn hoạt động" });
            var now = DateTime.Now;
            if (now < voucher.StartDate || now > voucher.EndDate) return BadRequest(new { message = "Voucher không trong thời gian hiệu lực" });
			Console.WriteLine($"[Voucher] Voucher active & valid period. Start={voucher.StartDate:O}, End={voucher.EndDate:O}, Percent={voucher.DiscountPercent}, Amount={voucher.DiscountAmount}");

            var userVouchers = await _userVoucherService.GetPagedFilteredAsync(new UserVoucherBM { UserId = request.UserId.Value, VoucherId = request.VoucherId.Value }, 1, 10);
            var userVoucher = userVouchers.Items.FirstOrDefault();
            if (userVoucher == null) return BadRequest(new { message = "Bạn không sở hữu voucher này" });
            if (userVoucher.IsUsed) return BadRequest(new { message = "Voucher đã được sử dụng" });
			Console.WriteLine($"[Voucher] UserVoucher found. IsUsed={userVoucher.IsUsed}, UserVoucherId={userVoucher.UserVoucherId}");

            var productVouchers = await _productVoucherService.GetPagedFilteredAsync(new ProductVoucherBM { VoucherId = request.VoucherId.Value }, 1, 1000);
            if (productVouchers.Items.Any())
            {
				Console.WriteLine($"[Voucher] Product-specific voucher. ProductVoucher count={productVouchers.Items.Count}");
                var cartProductIds = (await _cartService.GetByIdAsync(order.CartId.Value)).CartItems.Select(ci => ci.ProductId).Where(id => id.HasValue).Select(id => id.Value);
                var voucherProductIds = productVouchers.Items.Select(pv => pv.ProductId);
                var hasValidProduct = cartProductIds.Any(id => voucherProductIds.Contains(id));
                if (!hasValidProduct) return BadRequest(new { message = "Voucher không áp dụng cho sản phẩm trong giỏ hàng" });
				Console.WriteLine($"[Voucher] Matched products in cart: [{string.Join(", ", cartProductIds.Where(id => voucherProductIds.Contains(id)).Select(id => id.ToString()))}]");

                if (voucher.DiscountPercent.HasValue)
                {
                    var cartForVoucher = await _cartService.GetByIdAsync(order.CartId.Value);
                    var applicableItems = cartForVoucher.CartItems.Where(ci => ci.ProductId.HasValue && voucherProductIds.Contains(ci.ProductId.Value));
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
                var cartForVoucher = await _cartService.GetByIdAsync(order.CartId.Value);
                var totalCartAmount = cartForVoucher.CartItems.Sum(ci => ci.Price * ci.Quantity);
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

        var payment = await _service.CreateAsync(new PaymentBM
        {
            OrderId = order.OrderId,
            Amount = finalAmount,
            PaymentStatus = "Pending",
            PaymentDate = System.DateTime.Now
        });

        order.OrderStatus = "Pending";
        await _orderService.UpdateAsync(order.OrderId, order);

        // Lưu link voucher với order nhưng chưa đánh dấu IsUsed (inline)
        if (request.VoucherId.HasValue)
        {
            var linkUserVouchers = await _userVoucherService.GetPagedFilteredAsync(new UserVoucherBM { UserId = request.UserId.Value, VoucherId = request.VoucherId.Value, IsUsed = false }, 1, 10);
            var uv = linkUserVouchers.Items.FirstOrDefault();
            if (uv != null)
            {
                uv.OrderId = order.OrderId;
                await _userVoucherService.UpdateAsync(uv.UserVoucherId, uv);
            }
        }

        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var url = _vnPayService.CreatePaymentUrl(payment.PaymentId, order.OrderId, finalAmount, clientIp);
        return Ok(new { orderId = order.OrderId, paymentId = payment.PaymentId, originalAmount = amount, voucherDiscount = voucherDiscount, finalAmount = finalAmount, paymentUrl = url });
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


