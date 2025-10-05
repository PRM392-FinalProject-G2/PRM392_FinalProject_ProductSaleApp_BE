namespace ProductSaleApp.API.Models.RequestModel.Auth;

public class RegisterRequest
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}


