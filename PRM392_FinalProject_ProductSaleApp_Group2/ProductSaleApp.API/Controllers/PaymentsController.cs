using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;
    private readonly IMapper _mapper;
    private readonly IPaymentWorkflowService _paymentWorkflowService;
    private readonly IConfiguration _configuration;

    public PaymentsController(IPaymentService service, IMapper mapper, IPaymentWorkflowService paymentWorkflowService, IConfiguration configuration)
    {
        _service = service;
        _mapper = mapper;
        _paymentWorkflowService = paymentWorkflowService;
        _configuration = configuration;
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
        
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var result = await _paymentWorkflowService.CreateOrderAndPaymentAsync(orderModel, request.VoucherId, clientIp);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }
        
        return Ok(new { 
            orderId = result.OrderId, 
            paymentId = result.PaymentId, 
            originalAmount = result.OriginalAmount, 
            voucherDiscount = result.VoucherDiscount, 
            finalAmount = result.FinalAmount, 
            paymentUrl = result.PaymentUrl 
        });
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


