namespace ProductSaleApp.Service.BusinessModel;

public class LoginBM
{
    public string Identifier { get; set; }
    public string Password { get; set; }
}

public class RegisterBM
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}

public class AuthBM
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    public string AccessToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}


