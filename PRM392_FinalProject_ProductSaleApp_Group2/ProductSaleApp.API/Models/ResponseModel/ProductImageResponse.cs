namespace ProductSaleApp.API.Models.ResponseModel;

public class ProductImageResponse
{
    public int ImageId { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl { get; set; }
    public bool IsPrimary { get; set; }
}


