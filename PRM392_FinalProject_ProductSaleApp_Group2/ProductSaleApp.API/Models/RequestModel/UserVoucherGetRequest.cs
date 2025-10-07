namespace ProductSaleApp.API.Models.RequestModel;

public class UserVoucherGetRequest
{
    public int? UserVoucherId { get; set; }
    public int? UserId { get; set; }
    public int? VoucherId { get; set; }
    public bool? IsUsed { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


