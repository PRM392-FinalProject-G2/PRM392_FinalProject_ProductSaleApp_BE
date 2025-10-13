using System;

namespace ProductSaleApp.API.Models.ResponseModel;

public class WishlistMobileResponse
{
    public int WishlistId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string BriefDescription { get; set; }
    public decimal Price { get; set; }
    public string PrimaryImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}