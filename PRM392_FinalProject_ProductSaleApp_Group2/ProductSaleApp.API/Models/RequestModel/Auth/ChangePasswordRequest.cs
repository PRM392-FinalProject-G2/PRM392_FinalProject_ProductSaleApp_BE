using System.ComponentModel.DataAnnotations;

namespace ProductSaleApp.API.Models.RequestModel.Auth;

public class ChangePasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string ResetToken { get; set; }
    
    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; }
    
    [Required]
    [Compare("NewPassword", ErrorMessage = "Password and confirmation password do not match")]
    public string ConfirmPassword { get; set; }
}

