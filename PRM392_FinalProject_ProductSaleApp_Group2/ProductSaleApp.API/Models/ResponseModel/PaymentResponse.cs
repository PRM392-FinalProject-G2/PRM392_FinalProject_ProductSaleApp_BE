using System;

namespace ProductSaleApp.API.Models.ResponseModel;

public class PaymentResponse
{
    public int PaymentId { get; set; }
    public int? OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime PaymentDate { get; set; }
}


