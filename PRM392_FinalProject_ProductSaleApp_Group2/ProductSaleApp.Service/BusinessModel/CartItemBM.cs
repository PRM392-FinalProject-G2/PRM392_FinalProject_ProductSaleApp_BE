namespace ProductSaleApp.Service.BusinessModel;

public class CartItemBM
{
    public int CartItemId { get; set; }
    public int? CartId { get; set; }
    public int? ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public CartBM Cart { get; set; }
    public ProductBM Product { get; set; }
}


