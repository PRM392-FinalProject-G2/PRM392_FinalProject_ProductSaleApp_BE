namespace ProductSaleApp.API.Models.RequestModel;

public class ProductVoucherGetRequest
{
    public int? ProductVoucherId { get; set; }
    public int? ProductId { get; set; }
    public int? VoucherId { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


