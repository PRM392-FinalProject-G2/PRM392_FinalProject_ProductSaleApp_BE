namespace ProductSaleApp.API.Models.RequestModel;

public class CartRequest
{
    public int? UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
}


