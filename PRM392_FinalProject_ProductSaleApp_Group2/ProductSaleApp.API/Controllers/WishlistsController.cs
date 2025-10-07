using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WishlistsController : ControllerBase
{
    private readonly IWishlistService _service;
    private readonly IMapper _mapper;

    public WishlistsController(IWishlistService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<PagedResponse<WishlistResponse>>> GetFilter([FromQuery] WishlistGetRequest request)
    {
        var filter = _mapper.Map<WishlistBM>(request);
        var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
        return Ok(_mapper.Map<PagedResponse<WishlistResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WishlistResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<WishlistResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<WishlistResponse>> Create(WishlistRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<WishlistBM>(request));
        var response = _mapper.Map<WishlistResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.WishlistId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WishlistResponse>> Update(int id, WishlistRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<WishlistBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<WishlistResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}



