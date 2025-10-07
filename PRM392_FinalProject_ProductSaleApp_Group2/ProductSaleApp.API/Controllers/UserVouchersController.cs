using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserVouchersController : ControllerBase
{
    private readonly IUserVoucherService _service;
    private readonly IMapper _mapper;

    public UserVouchersController(IUserVoucherService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<PagedResponse<UserVoucherResponse>>> GetFilter([FromQuery] UserVoucherGetRequest request)
    {
        var filter = _mapper.Map<UserVoucherBM>(request);
        var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
        return Ok(_mapper.Map<PagedResponse<UserVoucherResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserVoucherResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<UserVoucherResponse>(item));
    }

    [HttpPost]
    public async Task<ActionResult<UserVoucherResponse>> Create(UserVoucherRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<UserVoucherBM>(request));
        var response = _mapper.Map<UserVoucherResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.UserVoucherId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserVoucherResponse>> Update(int id, UserVoucherRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<UserVoucherBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<UserVoucherResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}



