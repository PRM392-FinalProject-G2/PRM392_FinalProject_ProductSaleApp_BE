using System.ComponentModel.DataAnnotations;

namespace ProductSaleApp.API.Models.RequestModel.Auth;

public class VerifyOtpRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string Otp { get; set; }
}

