using System;

namespace ProductSaleApp.Service.BusinessModel;

public class ChatMessageBM
{
    public int ChatMessageId { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Message { get; set; }
    public DateTime SentAt { get; set; }

    public UserBM Sender { get; set; }
    public UserBM Receiver { get; set; }
}


