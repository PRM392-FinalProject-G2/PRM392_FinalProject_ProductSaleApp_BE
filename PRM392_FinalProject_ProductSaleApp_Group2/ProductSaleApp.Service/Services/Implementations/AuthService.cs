using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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

    public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = configuration;
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
            Role = "User"
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


