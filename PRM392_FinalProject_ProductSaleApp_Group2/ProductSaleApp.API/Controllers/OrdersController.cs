using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;
    private readonly IMapper _mapper;

    public OrdersController(IOrderService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<OrderResponse>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetPagedAsync(pageNumber, pageSize);
        return Ok(_mapper.Map<PagedResponse<OrderResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<OrderResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(OrderRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<OrderBM>(request));
        var response = _mapper.Map<OrderResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.OrderId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrderResponse>> Update(int id, OrderRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<OrderBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<OrderResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}


