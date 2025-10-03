using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductVouchersController : ControllerBase
{
    private readonly IProductVoucherService _service;
    private readonly IMapper _mapper;

    public ProductVouchersController(IProductVoucherService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductVoucherResponse>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetPagedAsync(pageNumber, pageSize);
        return Ok(_mapper.Map<PagedResponse<ProductVoucherResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductVoucherResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<ProductVoucherResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<ProductVoucherResponse>> Create(ProductVoucherRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<ProductVoucherBM>(request));
        var response = _mapper.Map<ProductVoucherResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.ProductVoucherId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductVoucherResponse>> Update(int id, ProductVoucherRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<ProductVoucherBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<ProductVoucherResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}



