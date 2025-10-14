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
    private readonly IVoucherService _voucherService;
    private readonly IUserVoucherService _userVoucherService;
    private readonly IProductVoucherService _productVoucherService;

    public PaymentsController(IPaymentService service, IMapper mapper, IVnPayService vnPayService, IOrderService orderService, ICartService cartService, IConfiguration configuration, IProductService productService, IVoucherService voucherService, IUserVoucherService userVoucherService, IProductVoucherService productVoucherService)
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
        
        Console.WriteLine($"VnPayCallback: PaymentId={paymentId}, OrderId={orderId}, Status={status}");

        if (status == "00")
        {
            payment.PaymentStatus = "Success";
            if (order != null) order.OrderStatus = "Paid";

            // Tăng popularity cho các product trong cart khi thanh toán thành công
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
                        Console.WriteLine($"VnPayCallback: Increased popularity for {productIds.Count} products: [{string.Join(", ", productIds)}]");
                    }
                }
            }

            // Inline: mark voucher used for this order
            if (order?.UserId.HasValue == true)
            {
                Console.WriteLine($"VnPayCallback: Looking for UserVoucher - UserId={order.UserId.Value}, OrderId={order.OrderId}");
                
                // Tìm UserVoucher cụ thể theo UserId và OrderId
                var uv = await _userVoucherService.GetByUserIdAndOrderIdAsync(order.UserId.Value, order.OrderId);
                
                Console.WriteLine($"VnPayCallback: Found UserVoucher: {uv != null}");
                if (uv != null)
                {
                    Console.WriteLine($"VnPayCallback: BEFORE UPDATE - IsUsed={uv.IsUsed}, UsedAt={uv.UsedAt}, UserVoucherId={uv.UserVoucherId}");
                    
                    // Chỉ update nếu chưa sử dụng
                    if (!uv.IsUsed)
                    {
                        uv.IsUsed = true;
                        uv.UsedAt = DateTime.Now;
                        
                        Console.WriteLine($"VnPayCallback: AFTER SET - IsUsed={uv.IsUsed}, UsedAt={uv.UsedAt}");
                        
                        var result = await _userVoucherService.UpdateAsync(uv.UserVoucherId, uv);
                        
                        Console.WriteLine($"VnPayCallback: UPDATE RESULT - IsUsed={result?.IsUsed}, UsedAt={result?.UsedAt}");
                    }
                    else
                    {
                        Console.WriteLine($"VnPayCallback: UserVoucher {uv.UserVoucherId} already used, skipping update");
                    }
                }
                else
                {
                    Console.WriteLine($"VnPayCallback: No UserVoucher found for OrderId={order.OrderId}");
                }
            }
        }
        else
        {
            payment.PaymentStatus = "Failed";
            if (order != null) order.OrderStatus = "Failed";

            // Inline: clear OrderId on failed payment so voucher remains reusable
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

        // Validate voucher (inline) và tính finalAmount
        decimal voucherDiscount = 0m;
        if (request.VoucherId.HasValue)
        {
            var voucher = await _voucherService.GetByIdAsync(request.VoucherId.Value);
            if (voucher == null) return BadRequest(new { message = "Voucher không tồn tại" });
            if (!voucher.IsActive) return BadRequest(new { message = "Voucher không còn hoạt động" });
            var now = DateTime.Now;
            if (now < voucher.StartDate || now > voucher.EndDate) return BadRequest(new { message = "Voucher không trong thời gian hiệu lực" });

            var userVouchers = await _userVoucherService.GetPagedFilteredAsync(new UserVoucherBM { UserId = request.UserId.Value, VoucherId = request.VoucherId.Value }, 1, 10);
            var userVoucher = userVouchers.Items.FirstOrDefault();
            if (userVoucher == null) return BadRequest(new { message = "Bạn không sở hữu voucher này" });
            if (userVoucher.IsUsed) return BadRequest(new { message = "Voucher đã được sử dụng" });

            var productVouchers = await _productVoucherService.GetPagedFilteredAsync(new ProductVoucherBM { VoucherId = request.VoucherId.Value }, 1, 1000);
            if (productVouchers.Items.Any())
            {
                var cartProductIds = (await _cartService.GetByIdAsync(order.CartId.Value)).CartItems.Select(ci => ci.ProductId).Where(id => id.HasValue).Select(id => id.Value);
                var voucherProductIds = productVouchers.Items.Select(pv => pv.ProductId);
                var hasValidProduct = cartProductIds.Any(id => voucherProductIds.Contains(id));
                if (!hasValidProduct) return BadRequest(new { message = "Voucher không áp dụng cho sản phẩm trong giỏ hàng" });

                if (voucher.DiscountPercent.HasValue)
                {
                    var cartForVoucher = await _cartService.GetByIdAsync(order.CartId.Value);
                    var applicableItems = cartForVoucher.CartItems.Where(ci => ci.ProductId.HasValue && voucherProductIds.Contains(ci.ProductId.Value));
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
                var cartForVoucher = await _cartService.GetByIdAsync(order.CartId.Value);
                var totalCartAmount = cartForVoucher.CartItems.Sum(ci => ci.Price * ci.Quantity);
                if (voucher.DiscountPercent.HasValue)
                {
                    voucherDiscount = totalCartAmount * (voucher.DiscountPercent.Value / 100);
                }
                else if (voucher.DiscountAmount.HasValue)
                {
                    voucherDiscount = voucher.DiscountAmount.Value;
                }
            }
        }

        var finalAmount = amount - voucherDiscount;

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


