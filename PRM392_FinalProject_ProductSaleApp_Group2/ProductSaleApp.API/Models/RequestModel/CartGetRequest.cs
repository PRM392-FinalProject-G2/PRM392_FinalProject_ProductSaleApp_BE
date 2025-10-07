namespace ProductSaleApp.API.Models.RequestModel;

public class CartGetRequest
{
    public int? CartId { get; set; }
    public int? UserId { get; set; }
    public string? Status { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


