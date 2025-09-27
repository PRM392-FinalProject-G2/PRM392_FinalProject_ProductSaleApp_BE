using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartItemsController : ControllerBase
{
    private readonly ICartItemService _service;
    private readonly IMapper _mapper;

    public CartItemsController(ICartItemService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<CartItemResponse>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetPagedAsync(pageNumber, pageSize);
        return Ok(_mapper.Map<PagedResponse<CartItemResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CartItemResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<CartItemResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<CartItemResponse>> Create(CartItemRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<CartItemBM>(request));
        var response = _mapper.Map<CartItemResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.CartItemId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CartItemResponse>> Update(int id, CartItemRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<CartItemBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<CartItemResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}


