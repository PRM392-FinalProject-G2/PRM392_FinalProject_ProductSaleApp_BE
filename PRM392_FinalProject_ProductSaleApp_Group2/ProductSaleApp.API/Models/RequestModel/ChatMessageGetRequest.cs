namespace ProductSaleApp.API.Models.RequestModel;

public class ChatMessageGetRequest
{
    public int? ChatMessageId { get; set; }
    public int? SenderId { get; set; }
    public int? ReceiverId { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


