using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    private readonly IMapper _mapper;

    public NotificationsController(INotificationService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<NotificationResponse>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetPagedAsync(pageNumber, pageSize);
        return Ok(_mapper.Map<PagedResponse<NotificationResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NotificationResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<NotificationResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<NotificationResponse>> Create(NotificationRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<NotificationBM>(request));
        var response = _mapper.Map<NotificationResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.NotificationId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<NotificationResponse>> Update(int id, NotificationRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<NotificationBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<NotificationResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}


