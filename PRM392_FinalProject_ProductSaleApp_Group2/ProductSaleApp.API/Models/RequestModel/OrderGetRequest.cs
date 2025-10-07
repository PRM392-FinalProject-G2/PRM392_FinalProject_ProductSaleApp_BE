namespace ProductSaleApp.API.Models.RequestModel;

public class OrderGetRequest
{
    public int? OrderId { get; set; }
    public int? UserId { get; set; }
    public int? CartId { get; set; }
    public string? OrderStatus { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


