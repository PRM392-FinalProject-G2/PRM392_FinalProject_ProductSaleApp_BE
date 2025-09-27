using System;

namespace ProductSaleApp.Service.BusinessModel;

public class PaymentBM
{
    public int PaymentId { get; set; }
    public int? OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime PaymentDate { get; set; }

    public OrderBM Order { get; set; }
}


