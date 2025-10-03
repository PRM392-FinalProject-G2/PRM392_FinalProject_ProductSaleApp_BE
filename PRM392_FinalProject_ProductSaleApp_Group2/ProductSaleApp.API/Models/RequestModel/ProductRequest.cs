namespace ProductSaleApp.API.Models.RequestModel;

public class ProductRequest
{
    public string ProductName { get; set; }
    public string BriefDescription { get; set; }
    public string FullDescription { get; set; }
    public string TechnicalSpecifications { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
}


