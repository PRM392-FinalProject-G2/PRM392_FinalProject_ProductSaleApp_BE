namespace ProductSaleApp.API.Models.ResponseModel.Auth;

public class ResetTokenResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string ResetToken { get; set; }
    public int ExpiresInSeconds { get; set; }
}

