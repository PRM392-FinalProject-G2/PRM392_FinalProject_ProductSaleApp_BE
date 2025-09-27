using System;

namespace ProductSaleApp.Service.BusinessModel;

public class ChatMessageBM
{
    public int ChatMessageId { get; set; }
    public int? UserId { get; set; }
    public string Message { get; set; }
    public DateTime SentAt { get; set; }

    public UserBM User { get; set; }
}


