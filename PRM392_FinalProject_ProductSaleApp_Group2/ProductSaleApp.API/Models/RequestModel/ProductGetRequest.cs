namespace ProductSaleApp.API.Models.RequestModel;

public class ProductGetRequest
{
    public int? ProductId { get; set; }
    public string? Search { get; set; }
    public List<int>? CategoryIds { get; set; }

    public List<int>? BrandIds { get; set; }

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public double? AverageRating { get; set; }

    public string? SortBy { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}



