using System;

namespace ProductSaleApp.Service.BusinessModel;

public class NotificationBM
{
    public int NotificationId { get; set; }
    public int? UserId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserBM User { get; set; }
}


