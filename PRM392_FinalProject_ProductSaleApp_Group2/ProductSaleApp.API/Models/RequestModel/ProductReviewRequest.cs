namespace ProductSaleApp.API.Models.RequestModel;

public class ProductReviewRequest
{
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public short Rating { get; set; }
    public string Comment { get; set; }
}


