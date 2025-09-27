using System;
using System.Collections.Generic;

namespace ProductSaleApp.API.Models.ResponseModel;

public class OrderResponse
{
    public int OrderId { get; set; }
    public int? CartId { get; set; }
    public int? UserId { get; set; }
    public string PaymentMethod { get; set; }
    public string BillingAddress { get; set; }
    public string OrderStatus { get; set; }
    public DateTime OrderDate { get; set; }

    public UserResponse User { get; set; }
    public CartResponse Cart { get; set; }
    public IReadOnlyList<PaymentResponse> Payments { get; set; }
}


