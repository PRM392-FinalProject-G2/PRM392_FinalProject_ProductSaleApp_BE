using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartService _service;
    private readonly IMapper _mapper;

    public CartsController(ICartService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<PagedResponse<CartResponse>>> GetFilter([FromQuery] CartGetRequest request)
    {
        var filter = _mapper.Map<CartBM>(request);
        var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
        return Ok(_mapper.Map<PagedResponse<CartResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CartResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<CartResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<CartResponse>> Create(CartRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<CartBM>(request));
        var response = _mapper.Map<CartResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.CartId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CartResponse>> Update(int id, CartRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<CartBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<CartResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}


