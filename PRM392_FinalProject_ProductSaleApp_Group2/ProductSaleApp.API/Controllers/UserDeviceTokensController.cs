using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserDeviceTokensController : ControllerBase
{
    private readonly IUserDeviceTokenService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<UserDeviceTokensController> _logger;

    public UserDeviceTokensController(
        IUserDeviceTokenService service, 
        IMapper mapper,
        ILogger<UserDeviceTokensController> logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Register or update FCM token for a user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<UserDeviceTokenResponse>> RegisterToken([FromBody] UserDeviceTokenRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _service.RegisterOrUpdateTokenAsync(request.UserId, request.FcmToken);
            var response = _mapper.Map<UserDeviceTokenResponse>(token);
            
            _logger.LogInformation($"Successfully registered FCM token for user {request.UserId}");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error registering FCM token for user {request.UserId}");
            return StatusCode(500, new { message = "Failed to register FCM token", error = ex.Message });
        }
    }

    /// <summary>
    /// Get active tokens for a user
    /// </summary>
    [HttpGet("user/{userId:int}/active")]
    public async Task<ActionResult<List<UserDeviceTokenResponse>>> GetActiveTokensByUserId(int userId)
    {
        try
        {
            var tokens = await _service.GetActiveTokensByUserIdAsync(userId);
            var response = _mapper.Map<List<UserDeviceTokenResponse>>(tokens);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting active tokens for user {userId}");
            return StatusCode(500, new { message = "Failed to get active tokens", error = ex.Message });
        }
    }

    /// <summary>
    /// Deactivate a specific token
    /// </summary>
    [HttpPost("deactivate")]
    public async Task<ActionResult> DeactivateToken([FromBody] UserDeviceTokenRequest request)
    {
        try
        {
            var success = await _service.DeactivateTokenAsync(request.UserId, request.FcmToken);
            
            if (success)
            {
                _logger.LogInformation($"Successfully deactivated FCM token for user {request.UserId}");
                return Ok(new { message = "Token deactivated successfully" });
            }
            
            return NotFound(new { message = "Token not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deactivating FCM token for user {request.UserId}");
            return StatusCode(500, new { message = "Failed to deactivate token", error = ex.Message });
        }
    }

    /// <summary>
    /// Deactivate all tokens for a user (e.g., on logout)
    /// </summary>
    [HttpPost("user/{userId:int}/deactivate-all")]
    public async Task<ActionResult> DeactivateAllUserTokens(int userId)
    {
        try
        {
            var success = await _service.DeactivateAllUserTokensAsync(userId);
            
            if (success)
            {
                _logger.LogInformation($"Successfully deactivated all FCM tokens for user {userId}");
                return Ok(new { message = "All tokens deactivated successfully" });
            }
            
            return BadRequest(new { message = "Failed to deactivate tokens" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deactivating all FCM tokens for user {userId}");
            return StatusCode(500, new { message = "Failed to deactivate all tokens", error = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific token by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDeviceTokenResponse>> GetById(int id)
    {
        try
        {
            var token = await _service.GetByIdAsync(id, includeDetails: true);
            
            if (token == null)
                return NotFound(new { message = "Token not found" });
            
            var response = _mapper.Map<UserDeviceTokenResponse>(token);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting token {id}");
            return StatusCode(500, new { message = "Failed to get token", error = ex.Message });
        }
    }
}


