namespace ProductSaleApp.API.Models.RequestModel;

public class NotificationGetRequest
{
    public int? NotificationId { get; set; }
    public int? UserId { get; set; }
    public bool? IsRead { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


