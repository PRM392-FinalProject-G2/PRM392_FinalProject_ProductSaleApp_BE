namespace ProductSaleApp.API.Models.RequestModel;

public class CartItemGetRequest
{
    public int? CartItemId { get; set; }
    public int? CartId { get; set; }
    public int? ProductId { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


