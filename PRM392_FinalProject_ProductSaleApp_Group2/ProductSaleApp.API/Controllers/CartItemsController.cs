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
    private readonly ICartService _cartService;
    private readonly IFirebaseNotificationService _firebaseService;
    private readonly IMapper _mapper;
    private readonly ILogger<CartItemsController> _logger;

    public CartItemsController(
        ICartItemService service, 
        ICartService cartService,
        IFirebaseNotificationService firebaseService,
        IMapper mapper,
        ILogger<CartItemsController> logger)
    {
        _service = service;
        _cartService = cartService;
        _firebaseService = firebaseService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<PagedResponse<CartItemResponse>>> GetFilter([FromQuery] CartItemGetRequest request)
    {
        var filter = _mapper.Map<CartItemBM>(request);
        var paged = await _service.GetPagedFilteredAsync(filter, request.PageNumber, request.PageSize);
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
        var created = await _service.CreateCartItemAsync(_mapper.Map<CartItemBM>(request));
        var response = _mapper.Map<CartItemResponse>(created);
        
        // Send notification after adding item to cart
        await SendCartUpdateNotification(request.CartId);
        
        return CreatedAtAction(nameof(GetById), new { id = response.CartItemId }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CartItemResponse>> Update(int id, CartItemRequest request)
    {
        var updated = await _service.UpdateAsync(id, _mapper.Map<CartItemBM>(request));
        if (updated == null) return NotFound();
        
        // Send notification after updating cart item
        await SendCartUpdateNotification(request.CartId);
        
        return Ok(_mapper.Map<CartItemResponse>(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Get cart item before deleting to know which cart to update
        var item = await _service.GetByIdAsync(id, includeDetails: false);
        int? cartId = item?.CartId;
        
        var ok = await _service.DeleteCartItemAsync(id);
        if (!ok) return NotFound();
        
        // Send notification after deleting cart item
        if (cartId.HasValue)
        {
            await SendCartUpdateNotification(cartId.Value);
        }
        
        return NoContent();
    }

    private async Task SendCartUpdateNotification(int cartId)
    {
        try
        {
            // Get cart with items to calculate count
            var cart = await _cartService.GetByIdAsync(cartId, includeDetails: true);
            if (cart?.UserId != null)
            {
                var itemCount = cart.CartItems?.Count ?? 0;
                await _firebaseService.SendCartUpdateNotificationAsync(cart.UserId.Value, itemCount);
                _logger.LogInformation($"Sent cart update notification to user {cart.UserId}, item count: {itemCount}");
            }
        }
        catch (Exception ex)
        {
            // Don't fail the request if notification fails
            _logger.LogError(ex, $"Failed to send cart update notification for cart {cartId}");
        }
    }

}


