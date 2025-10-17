namespace ProductSaleApp.Service.BusinessModel;

public class AmountCalculationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal VoucherDiscount { get; set; }
    public decimal FinalAmount { get; set; }
}




