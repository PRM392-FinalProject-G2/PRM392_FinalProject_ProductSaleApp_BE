namespace ProductSaleApp.API.Models.RequestModel;

public class PaymentGetRequest
{
    public int? PaymentId { get; set; }
    public int? OrderId { get; set; }
    public string? PaymentStatus { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


