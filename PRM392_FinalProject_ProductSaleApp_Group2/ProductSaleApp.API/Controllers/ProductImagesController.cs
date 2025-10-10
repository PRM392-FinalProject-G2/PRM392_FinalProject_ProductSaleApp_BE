using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductImagesController : ControllerBase
{
    private readonly IProductImageService _service;
    private readonly IMapper _mapper;

    public ProductImagesController(IProductImageService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductImageResponse>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var paged = await _service.GetPagedAsync(pageNumber, pageSize);
        return Ok(_mapper.Map<PagedResponse<ProductImageResponse>>(paged));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductImageResponse>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id, includeDetails: true);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<ProductImageResponse>(item));
    }

    [HttpGet("product/{productId:int}")]
    public async Task<ActionResult<List<ProductImageResponse>>> GetByProductId(int productId)
    {
        var items = await _service.GetByProductIdAsync(productId);
        return Ok(_mapper.Map<List<ProductImageResponse>>(items));
    }

    [HttpPost]
    public async Task<ActionResult<ProductImageResponse>> Create(ProductImageRequest request)
    {
        var created = await _service.CreateAsync(_mapper.Map<ProductImageBM>(request));
        var response = _mapper.Map<ProductImageResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.ImageId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductImageResponse>> Update(int id, ProductImageRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<ProductImageBM>(request));
        if (updated == null) return NotFound();
        return Ok(_mapper.Map<ProductImageResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}


