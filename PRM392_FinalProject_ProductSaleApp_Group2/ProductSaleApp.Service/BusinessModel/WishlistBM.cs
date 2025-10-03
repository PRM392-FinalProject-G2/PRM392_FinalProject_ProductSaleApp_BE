using System;

namespace ProductSaleApp.Service.BusinessModel;

public class WishlistBM
{
    public int WishlistId { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserBM User { get; set; }
    public ProductBM Product { get; set; }
}



