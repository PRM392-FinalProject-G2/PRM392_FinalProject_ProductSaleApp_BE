namespace ProductSaleApp.API.Models.RequestModel;

public class PaymentCalculateRequest
{
    public int? CartId { get; set; }
    public int? UserId { get; set; }
    public int? VoucherId { get; set; }
}




