namespace ProductSaleApp.Service.BusinessModel;

public class ConversationBM
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string UserAvatar { get; set; }
    public string LastMessage { get; set; }
    public DateTime LastMessageTime { get; set; }
    public int UnreadCount { get; set; }
    public bool IsOnline { get; set; }
    public int LastMessageSenderId { get; set; }
}
