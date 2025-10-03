using System;

namespace ProductSaleApp.API.Models.ResponseModel;

public class WishlistResponse
{
    public int WishlistId { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserResponse User { get; set; }
    public ProductResponse Product { get; set; }
}



