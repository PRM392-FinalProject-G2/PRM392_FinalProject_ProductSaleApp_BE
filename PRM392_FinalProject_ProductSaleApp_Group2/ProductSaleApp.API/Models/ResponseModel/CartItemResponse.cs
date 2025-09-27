namespace ProductSaleApp.API.Models.ResponseModel;

public class CartItemResponse
{
    public int CartItemId { get; set; }
    public int? CartId { get; set; }
    public int? ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public ProductResponse Product { get; set; }
}


