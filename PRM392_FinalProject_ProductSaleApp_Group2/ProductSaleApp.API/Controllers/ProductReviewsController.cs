using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductReviewsController : ControllerBase
{
    private readonly IProductReviewService _service;
    private readonly IMapper _mapper;

    public ProductReviewsController(IProductReviewService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductReviewResponse>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetPagedAsync(pageNumber, pageSize);
        return Ok(_mapper.Map<PagedResponse<ProductReviewResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductReviewResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<ProductReviewResponse>(item));
    }

    [HttpGet("product/{productId:int}")]
    public async Task<ActionResult<List<ProductReviewResponse>>> GetByProductId(int productId)
    {
        var items = await _service.GetByProductIdAsync(productId);
        return Ok(_mapper.Map<List<ProductReviewResponse>>(items));
    }

    [HttpGet("product/{productId:int}/paged")]
    public async Task<ActionResult<PagedResponse<ProductReviewResponse>>> GetPagedByProductId(int productId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetPagedByProductIdAsync(productId, pageNumber, pageSize);
        return Ok(_mapper.Map<PagedResponse<ProductReviewResponse>>(paged));
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<List<ProductReviewResponse>>> GetByUserId(int userId)
    {
        var items = await _service.GetByUserIdAsync(userId);
        return Ok(_mapper.Map<List<ProductReviewResponse>>(items));
    }

    [HttpPost]
    public async Task<ActionResult<ProductReviewResponse>> Create(ProductReviewRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<ProductReviewBM>(request));
        var response = _mapper.Map<ProductReviewResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.ReviewId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductReviewResponse>> Update(int id, ProductReviewRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<ProductReviewBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<ProductReviewResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}


