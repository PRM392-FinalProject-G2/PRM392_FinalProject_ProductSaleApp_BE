using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly IMapper _mapper;

    public UsersController(IUserService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<PagedResponse<UserResponse>>> GetFilter([FromQuery] UserGetRequest request)
    {
        var filter = _mapper.Map<UserBM>(request);
        var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
        return Ok(_mapper.Map<PagedResponse<UserResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<UserResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(UserRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<UserBM>(request));
        var response = _mapper.Map<UserResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.UserId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponse>> Update(int id, UserRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<UserBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<UserResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}


