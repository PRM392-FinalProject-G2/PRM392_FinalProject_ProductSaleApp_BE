using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreLocationsController : ControllerBase
{
    private readonly IStoreLocationService _service;
    private readonly IMapper _mapper;

    public StoreLocationsController(IStoreLocationService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<StoreLocationResponse>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetPagedAsync(pageNumber, pageSize);
        return Ok(_mapper.Map<PagedResponse<StoreLocationResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StoreLocationResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<StoreLocationResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<StoreLocationResponse>> Create(StoreLocationRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<StoreLocationBM>(request));
        var response = _mapper.Map<StoreLocationResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.LocationId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StoreLocationResponse>> Update(int id, StoreLocationRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<StoreLocationBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<StoreLocationResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}


