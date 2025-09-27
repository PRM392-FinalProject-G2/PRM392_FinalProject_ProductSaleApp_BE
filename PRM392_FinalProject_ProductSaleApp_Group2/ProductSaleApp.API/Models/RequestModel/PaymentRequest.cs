namespace ProductSaleApp.API.Models.RequestModel;

public class PaymentRequest
{
    public int? OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; }
}


