namespace ProductSaleApp.API.Models.RequestModel;

public class UserRequest
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Role { get; set; }
    public IFormFile? AvatarFile { get; set; }

}


