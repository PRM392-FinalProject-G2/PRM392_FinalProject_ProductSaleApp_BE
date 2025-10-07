using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _service;
    private readonly IMapper _mapper;

    public BrandsController(IBrandService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<PagedResponse<BrandResponse>>> GetFilter([FromQuery] BrandGetRequest request)
    {
        var filter = _mapper.Map<BrandBM>(request);
        var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
        return Ok(_mapper.Map<PagedResponse<BrandResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BrandResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<BrandResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<BrandResponse>> Create(BrandRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<BrandBM>(request));
        var response = _mapper.Map<BrandResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.BrandId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BrandResponse>> Update(int id, BrandRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<BrandBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<BrandResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}



