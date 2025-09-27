using System;

namespace ProductSaleApp.API.Models.ResponseModel;

public class ChatMessageResponse
{
    public int ChatMessageId { get; set; }
    public int? UserId { get; set; }
    public string Message { get; set; }
    public DateTime SentAt { get; set; }

    public UserResponse User { get; set; }
}


