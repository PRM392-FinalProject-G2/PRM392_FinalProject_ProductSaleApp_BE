using System;
using System.Collections.Generic;

namespace ProductSaleApp.Service.BusinessModel;

public class OrderBM
{
    public int OrderId { get; set; }
    public int? CartId { get; set; }
    public int? UserId { get; set; }
    public string PaymentMethod { get; set; }
    public string BillingAddress { get; set; }
    public string OrderStatus { get; set; }
    public DateTime OrderDate { get; set; }

    public CartBM Cart { get; set; }
    public UserBM User { get; set; }
    public IReadOnlyList<PaymentBM> Payments { get; set; }
}


