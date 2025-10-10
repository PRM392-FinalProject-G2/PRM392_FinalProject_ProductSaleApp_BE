using System;

namespace ProductSaleApp.Service.BusinessModel;

public class ProductReviewBM
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public short Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    // User info for display
    public UserBM User { get; set; }
}


