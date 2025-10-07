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

    [HttpPost("request-otp")]
    public async Task<ActionResult<OtpResponse>> RequestOtp(RequestOtpRequest request)
    {
        var result = await _authService.RequestOtpAsync(_mapper.Map<RequestOtpBM>(request));
        if (!result)
        {
            return BadRequest(new OtpResponse 
            { 
                Success = false, 
                Message = "Email does not match with the user ID or failed to send OTP" 
            });
        }
        
        return Ok(new OtpResponse 
        { 
            Success = true, 
            Message = "OTP has been sent to your email. It will expire in 5 minutes." 
        });
    }

    [HttpPost("verify-otp")]
    public async Task<ActionResult<ResetTokenResponse>> VerifyOtp(VerifyOtpRequest request)
    {
        var result = await _authService.VerifyOtpAsync(_mapper.Map<VerifyOtpBM>(request));
        
        if (!result.Success)
        {
            return BadRequest(_mapper.Map<ResetTokenResponse>(result));
        }
        
        return Ok(_mapper.Map<ResetTokenResponse>(result));
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<OtpResponse>> ChangePassword(ChangePasswordRequest request)
    {
        var result = await _authService.ChangePasswordAsync(_mapper.Map<ChangePasswordBM>(request));
        if (!result)
        {
            return BadRequest(new OtpResponse 
            { 
                Success = false, 
                Message = "Failed to change password. Please check your OTP and try again." 
            });
        }
        
        return Ok(new OtpResponse 
        { 
            Success = true, 
            Message = "Password changed successfully" 
        });
    }
}


