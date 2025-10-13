using System;

namespace ProductSaleApp.Service.BusinessModel;

public class WishlistMobileBM
{
    public int WishlistId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string BriefDescription { get; set; }
    public decimal Price { get; set; }
    public string PrimaryImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}