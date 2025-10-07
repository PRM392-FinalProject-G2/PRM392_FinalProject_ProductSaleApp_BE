namespace ProductSaleApp.API.Models.RequestModel;

public class BrandGetRequest
{
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public string? Description { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


