namespace ProductSaleApp.API.Models.ResponseModel.Auth;

public class AuthResponse
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    public string AccessToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}


