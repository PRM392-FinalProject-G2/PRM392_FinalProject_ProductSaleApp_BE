using System.Collections.Generic;

namespace ProductSaleApp.Service.BusinessModel;

public class CartBM
{
    public int CartId { get; set; }
    public int? UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }

    public UserBM User { get; set; }
    public IReadOnlyList<CartItemBM> CartItems { get; set; }
    public IReadOnlyList<OrderBM> Orders { get; set; }
}


