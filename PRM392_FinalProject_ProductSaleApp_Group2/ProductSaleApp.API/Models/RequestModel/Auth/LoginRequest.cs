namespace ProductSaleApp.API.Models.RequestModel.Auth;

public class LoginRequest
{
    public string Identifier { get; set; } // username or email or phone
    public string Password { get; set; }
}


