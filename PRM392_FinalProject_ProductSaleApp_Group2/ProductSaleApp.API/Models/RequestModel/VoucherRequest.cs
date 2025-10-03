using System;

namespace ProductSaleApp.API.Models.RequestModel;

public class VoucherRequest
{
    public string Code { get; set; }
    public string Description { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? DiscountAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}



