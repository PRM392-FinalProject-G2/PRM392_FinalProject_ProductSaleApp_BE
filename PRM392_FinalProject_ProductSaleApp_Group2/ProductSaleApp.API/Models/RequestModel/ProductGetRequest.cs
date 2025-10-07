namespace ProductSaleApp.API.Models.RequestModel;

public class ProductGetRequest
{
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


