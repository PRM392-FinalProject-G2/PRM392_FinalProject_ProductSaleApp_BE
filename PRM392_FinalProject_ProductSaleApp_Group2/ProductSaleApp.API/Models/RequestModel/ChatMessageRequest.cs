namespace ProductSaleApp.API.Models.RequestModel;

public class ChatMessageRequest
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Message { get; set; }
}


