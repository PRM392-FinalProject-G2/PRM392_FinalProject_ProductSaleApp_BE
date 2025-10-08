using System.Collections.Generic;

namespace ProductSaleApp.Service.BusinessModel;

public class UserBM
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Role { get; set; }
    public string Avatarurl { get; set; }


    public IReadOnlyList<CartBM> Carts { get; set; }
    public IReadOnlyList<ChatMessageBM> ChatMessages { get; set; }
    public IReadOnlyList<NotificationBM> Notifications { get; set; }
    public IReadOnlyList<OrderBM> Orders { get; set; }
}


