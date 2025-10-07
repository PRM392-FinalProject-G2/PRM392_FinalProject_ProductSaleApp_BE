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

    public PaymentsController(IPaymentService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
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


