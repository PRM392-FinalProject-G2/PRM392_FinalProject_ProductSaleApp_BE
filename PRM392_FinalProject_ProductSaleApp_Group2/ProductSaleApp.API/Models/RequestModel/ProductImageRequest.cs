namespace ProductSaleApp.API.Models.RequestModel;

public class ProductImageRequest
{
    public int ProductId { get; set; }
    public string ImageUrl { get; set; }
    public bool IsPrimary { get; set; }
}


