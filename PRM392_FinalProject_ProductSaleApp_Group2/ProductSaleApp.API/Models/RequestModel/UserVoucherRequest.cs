using System;

namespace ProductSaleApp.API.Models.RequestModel;

public class UserVoucherRequest
{
    public int UserId { get; set; }
    public int VoucherId { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public int? OrderId { get; set; }
}



