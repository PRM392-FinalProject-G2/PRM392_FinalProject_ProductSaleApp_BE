using System.Collections.Generic;

namespace ProductSaleApp.API.Models.ResponseModel;

public class CartResponse
{
    public int CartId { get; set; }
    public int? UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }

    public UserResponse User { get; set; }
    public IReadOnlyList<CartItemResponse> CartItems { get; set; }
}


