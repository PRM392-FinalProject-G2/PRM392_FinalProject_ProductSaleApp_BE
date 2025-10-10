using System;

namespace ProductSaleApp.API.Models.ResponseModel;

public class ProductReviewResponse
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string UserAvatarUrl { get; set; }
    public short Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}


