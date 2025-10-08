namespace ProductSaleApp.Service.BusinessModel;

public class ChangePasswordBM
{
    public string Email { get; set; }
    public string ResetToken { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}

