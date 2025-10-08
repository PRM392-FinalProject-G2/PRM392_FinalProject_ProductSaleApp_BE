using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly IEmailService _emailService;

    public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IMemoryCache cache, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = configuration;
        _cache = cache;
        _emailService = emailService;
    }

    public async Task<AuthBM> RegisterAsync(RegisterBM model)
    {
        // Uniqueness checks
        var existing = (await _unitOfWork.Repository<User>().GetAllAsync(u =>
            u.Username == model.Username || u.Email == model.Email || u.Phonenumber == model.PhoneNumber)).FirstOrDefault();
        if (existing != null) return null;

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            Phonenumber = model.PhoneNumber,
            Passwordhash = HashPassword(model.Password),
            Role = "Customer",
            Avatarurl = "https://res.cloudinary.com/dx3fdlq2p/image/upload/v1759829766/1053244_uifxod.png"
        };
        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return GenerateAuthResult(user);
    }

    public async Task<AuthBM> LoginAsync(LoginBM model)
    {
        var users = await _unitOfWork.Repository<User>().GetAllAsync(u =>
            u.Username == model.Identifier || u.Email == model.Identifier || u.Phonenumber == model.Identifier);
        var user = users.FirstOrDefault();
        if (user == null) return null;

        var hashed = HashPassword(model.Password);
        if (!TimingSafeEquals(user.Passwordhash, hashed)) return null;

        return GenerateAuthResult(user);
    }

    public Task<bool> LogoutAsync(int userId)
    {
        // Stateless JWT: nothing to do server-side
        return Task.FromResult(true);
    }

    public async Task<bool> RequestOtpAsync(RequestOtpBM model)
    {
        // Check if email matches the userId
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(model.UserId);
        if (user == null || user.Email != model.Email)
        {
            return false;
        }

        // Generate 4-digit OTP
        var random = new Random();
        var otp = random.Next(1000, 9999).ToString();

        // Store OTP in cache with 5 minutes expiration
        var cacheKey = $"OTP_{model.Email}";
        _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));

        // Send OTP via email
        var emailSent = await _emailService.SendOtpEmailAsync(model.Email, otp);
        
        return emailSent;
    }

    public Task<ResetTokenBM> VerifyOtpAsync(VerifyOtpBM model)
    {
        var cacheKey = $"OTP_{model.Email}";
        
        if (_cache.TryGetValue(cacheKey, out string cachedOtp))
        {
            if (cachedOtp == model.Otp)
            {
                // Generate reset token
                var resetToken = Guid.NewGuid().ToString("N");
                var resetTokenKey = $"RESET_TOKEN_{model.Email}";
                var expiresInSeconds = 600; // 10 minutes
                
                // Store reset token in cache
                _cache.Set(resetTokenKey, resetToken, TimeSpan.FromSeconds(expiresInSeconds));
                
                // Remove OTP from cache (one-time use)
                _cache.Remove(cacheKey);
                
                return Task.FromResult(new ResetTokenBM
                {
                    Success = true,
                    Message = "OTP verified successfully. You can now reset your password.",
                    ResetToken = resetToken,
                    ExpiresInSeconds = expiresInSeconds
                });
            }
        }

        return Task.FromResult(new ResetTokenBM
        {
            Success = false,
            Message = "Invalid or expired OTP",
            ResetToken = null,
            ExpiresInSeconds = 0
        });
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordBM model)
    {
        // Verify reset token
        var resetTokenKey = $"RESET_TOKEN_{model.Email}";
        
        if (!_cache.TryGetValue(resetTokenKey, out string cachedToken))
        {
            return false; // Token not found or expired
        }
        
        if (cachedToken != model.ResetToken)
        {
            return false; // Invalid token
        }

        // Check if passwords match
        if (model.NewPassword != model.ConfirmPassword)
        {
            return false;
        }

        // Find user by email
        var users = await _unitOfWork.Repository<User>().GetAllAsync(u => u.Email == model.Email);
        var user = users.FirstOrDefault();
        
        if (user == null)
        {
            return false;
        }

        // Update password
        user.Passwordhash = HashPassword(model.NewPassword);
        _unitOfWork.Repository<User>().Update(user);
        await _unitOfWork.SaveChangesAsync();

        // Remove reset token from cache after successful password change
        _cache.Remove(resetTokenKey);

        return true;
    }

    private string HashPassword(string password)
    {
        // Hash password with HMACSHA256 using Jwt:Key as secret
        var secret = _configuration["Jwt:Key"] ?? string.Empty;
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }

    private bool TimingSafeEquals(string a, string b)
    {
        var ba = Encoding.UTF8.GetBytes(a ?? string.Empty);
        var bb = Encoding.UTF8.GetBytes(b ?? string.Empty);
        if (ba.Length != bb.Length) return false;
        var result = 0;
        for (int i = 0; i < ba.Length; i++) result |= ba[i] ^ bb[i];
        return result == 0;
    }

    private AuthBM GenerateAuthResult(User user)
    {
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60"));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Userid.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            },
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthBM
        {
            UserId = user.Userid,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.Phonenumber,
            Role = user.Role,
            AccessToken = tokenString,
            ExpiresAt = expires
        };
    }
}


