namespace ProductSaleApp.API.Models.RequestModel;

public class ProductImageGetRequest
{
    public int? ImageId { get; set; }
    public int? ProductId { get; set; }
    public bool? IsPrimary { get; set; }
}


