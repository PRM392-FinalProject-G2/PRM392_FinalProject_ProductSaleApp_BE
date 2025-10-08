namespace ProductSaleApp.Service.BusinessModel;

public class ResetTokenBM
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string ResetToken { get; set; }
    public int ExpiresInSeconds { get; set; }
}

