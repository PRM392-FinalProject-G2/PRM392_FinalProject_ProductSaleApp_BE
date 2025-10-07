namespace ProductSaleApp.API.Models.RequestModel;

public class CategoryGetRequest
{
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


