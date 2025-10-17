using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.API.Models.RequestModel;

public class SendMessageRequest
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Message { get; set; }
}

public class GetChatHistoryRequest
{
    public int UserId { get; set; }
    public int OtherUserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class GetConversationsRequest
{
    public int UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
