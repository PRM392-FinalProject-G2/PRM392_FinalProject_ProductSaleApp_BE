namespace ProductSaleApp.API.Models.ResponseModel;

public class ChatHistoryResponse
{
    public int ChatMessageId { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Message { get; set; }
    public DateTime SentAt { get; set; }
    public string SenderName { get; set; }
    public string SenderAvatar { get; set; }
    public string ReceiverName { get; set; }
    public string ReceiverAvatar { get; set; }
}

public class ConversationResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string UserAvatar { get; set; }
    public string LastMessage { get; set; }
    public DateTime LastMessageTime { get; set; }
    public int UnreadCount { get; set; }
    public bool IsOnline { get; set; }
}

public class MessageSentResponse
{
    public bool Success { get; set; }
    public int ChatMessageId { get; set; }
    public string Message { get; set; }
}
