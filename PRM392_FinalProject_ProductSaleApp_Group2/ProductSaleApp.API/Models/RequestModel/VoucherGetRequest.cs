namespace ProductSaleApp.API.Models.RequestModel;

public class VoucherGetRequest
{
    public int? VoucherId { get; set; }
    public string? Code { get; set; }
    public bool? IsActive { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


