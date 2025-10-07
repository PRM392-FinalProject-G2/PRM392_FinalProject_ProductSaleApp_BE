using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VouchersController : ControllerBase
{
    private readonly IVoucherService _service;
    private readonly IMapper _mapper;

    public VouchersController(IVoucherService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<PagedResponse<VoucherResponse>>> GetFilter([FromQuery] VoucherGetRequest request)
    {
        var filter = _mapper.Map<VoucherBM>(request);
        var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
        return Ok(_mapper.Map<PagedResponse<VoucherResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VoucherResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<VoucherResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<VoucherResponse>> Create(VoucherRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<VoucherBM>(request));
        var response = _mapper.Map<VoucherResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.VoucherId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<VoucherResponse>> Update(int id, VoucherRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<VoucherBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<VoucherResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}



