namespace ProductSaleApp.API.Models.RequestModel;

public class PaymentCreateFinalRequest
{
    public int? CartId { get; set; }
    public int? UserId { get; set; }
    public int? VoucherId { get; set; }
    public decimal FinalAmount { get; set; }
    public string PaymentMethod { get; set; }
    public string BillingAddress { get; set; }
}


