using System;

namespace ProductSaleApp.Service.BusinessModel;

public class UserVoucherBM
{
    public int UserVoucherId { get; set; }
    public int UserId { get; set; }
    public int VoucherId { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public int? OrderId { get; set; }
    public DateTime AssignedAt { get; set; }

    public UserBM User { get; set; }
    public VoucherBM Voucher { get; set; }
    public OrderBM Order { get; set; }
}



