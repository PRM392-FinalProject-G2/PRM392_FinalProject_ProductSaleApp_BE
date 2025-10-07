namespace ProductSaleApp.API.Models.RequestModel;

public class WishlistGetRequest
{
    public int? WishlistId { get; set; }
    public int? UserId { get; set; }
    public int? ProductId { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


