using System.Threading.Tasks;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IFirebaseNotificationService
{
    Task<bool> SendNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string> data = null);
    Task<bool> SendNotificationToMultipleDevicesAsync(List<string> fcmTokens, string title, string body, Dictionary<string, string> data = null);
    Task<bool> SendCartUpdateNotificationAsync(int userId, int cartItemCount);
    Task<bool> SendOrderNotificationAsync(int userId, string orderStatus, string orderDetails);
}


