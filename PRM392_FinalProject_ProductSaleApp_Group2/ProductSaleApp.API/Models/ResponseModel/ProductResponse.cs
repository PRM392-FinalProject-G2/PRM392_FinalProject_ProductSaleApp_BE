using System.Collections.Generic;

namespace ProductSaleApp.API.Models.ResponseModel;

public class ProductResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string BriefDescription { get; set; }
    public string FullDescription { get; set; }
    public string TechnicalSpecifications { get; set; }
    public decimal Price { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int Popularity { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }

    public CategoryResponse Category { get; set; }
    public BrandResponse Brand { get; set; }
    public List<ProductImageResponse> ProductImages { get; set; }
    public List<ProductReviewResponse> ProductReviews { get; set; }
}


