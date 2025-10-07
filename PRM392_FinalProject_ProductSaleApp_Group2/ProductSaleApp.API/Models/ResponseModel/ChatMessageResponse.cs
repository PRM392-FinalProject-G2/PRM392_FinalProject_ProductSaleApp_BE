using System;

namespace ProductSaleApp.API.Models.ResponseModel;

public class ChatMessageResponse
{
    public int ChatMessageId { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Message { get; set; }
    public DateTime SentAt { get; set; }

    public UserResponse Sender { get; set; }
    public UserResponse Receiver { get; set; }
}


