using System.Collections.Generic;

namespace ProductSaleApp.Service.BusinessModel;

public class ProductBM
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

    public CategoryBM Category { get; set; }
    public BrandBM Brand { get; set; }
    public IReadOnlyList<ProductImageBM> ProductImages { get; set; }
    public IReadOnlyList<ProductReviewBM> ProductReviews { get; set; }
}


