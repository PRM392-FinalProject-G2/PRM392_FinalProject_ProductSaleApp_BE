namespace ProductSaleApp.API.Models.RequestModel;

public class ProductReviewGetRequest
{
    public int? ReviewId { get; set; }
    public int? ProductId { get; set; }
    public int? UserId { get; set; }
    public short? Rating { get; set; }
}


