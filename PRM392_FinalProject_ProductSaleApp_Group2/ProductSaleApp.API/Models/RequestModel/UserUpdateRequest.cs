using System.ComponentModel.DataAnnotations;

namespace ProductSaleApp.API.Models.RequestModel;

public class UserUpdateRequest
{
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string? Email { get; set; }
    
    [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng (phải là số điện thoại Việt Nam)")]
    public string? PhoneNumber { get; set; }
    
    public string? Address { get; set; }
    public IFormFile? AvatarFile { get; set; }
}

