using System.ComponentModel.DataAnnotations;

namespace ProductSaleApp.API.Models.RequestModel.Auth;

public class RequestOtpRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public int UserId { get; set; }
}

