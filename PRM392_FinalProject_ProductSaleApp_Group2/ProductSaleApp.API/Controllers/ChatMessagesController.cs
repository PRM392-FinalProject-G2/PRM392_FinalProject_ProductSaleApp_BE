using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatMessagesController : ControllerBase
{
    private readonly IChatMessageService _service;
    private readonly IMapper _mapper;

    public ChatMessagesController(IChatMessageService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ChatMessageResponse>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetPagedAsync(pageNumber, pageSize);
        return Ok(_mapper.Map<PagedResponse<ChatMessageResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ChatMessageResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<ChatMessageResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<ChatMessageResponse>> Create(ChatMessageRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<ChatMessageBM>(request));
        var response = _mapper.Map<ChatMessageResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.ChatMessageId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ChatMessageResponse>> Update(int id, ChatMessageRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<ChatMessageBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<ChatMessageResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}


