using System;

namespace ProductSaleApp.API.Models.ResponseModel;

public class NotificationResponse
{
    public int NotificationId { get; set; }
    public int? UserId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }

    public UserResponse User { get; set; }
}


