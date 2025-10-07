namespace ProductSaleApp.API.Models.RequestModel;

public class UserGetRequest
{
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


