namespace ProductSaleApp.Service.BusinessModel;

public class CreateOrderPaymentResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int OrderId { get; set; }
    public int PaymentId { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal VoucherDiscount { get; set; }
    public decimal FinalAmount { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
}
