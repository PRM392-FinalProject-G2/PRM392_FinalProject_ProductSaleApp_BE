using System;

namespace ProductSaleApp.API.Models.ResponseModel;

public class UserVoucherResponse
{
    public int UserVoucherId { get; set; }
    public int UserId { get; set; }
    public int VoucherId { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public int? OrderId { get; set; }
    public DateTime AssignedAt { get; set; }

    public UserResponse User { get; set; }
    public VoucherResponse Voucher { get; set; }
    public OrderResponse Order { get; set; }
}



