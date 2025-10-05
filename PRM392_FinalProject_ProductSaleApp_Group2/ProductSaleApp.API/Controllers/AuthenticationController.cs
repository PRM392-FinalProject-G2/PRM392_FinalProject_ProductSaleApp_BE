using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSaleApp.API.Models.RequestModel.Auth;
using ProductSaleApp.API.Models.ResponseModel.Auth;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public AuthenticationController(IAuthService authService, IMapper mapper)
    {
        _authService = authService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(_mapper.Map<RegisterBM>(request));
        if (result == null) return Conflict("Username/Email/Phone already exists");
        return Ok(_mapper.Map<AuthResponse>(result));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(_mapper.Map<LoginBM>(request));
        if (result == null) return Unauthorized();
        return Ok(_mapper.Map<AuthResponse>(result));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromQuery] int userId)
    {
        var ok = await _authService.LogoutAsync(userId);
        if (!ok) return BadRequest();
        return NoContent();
    }
}


