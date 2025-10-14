namespace ProductSaleApp.Service.BusinessModel;

public class VnPayCallbackResult
{
	public bool Success { get; set; }
	public int OrderId { get; set; }
	public decimal Amount { get; set; }
	public string Message { get; set; } = string.Empty;
}


