using System;

namespace ProductSaleApp.Service.BusinessModel;

public class VoucherBM
{
    public int VoucherId { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? DiscountAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}



