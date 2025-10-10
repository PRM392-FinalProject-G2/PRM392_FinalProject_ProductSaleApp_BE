using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IUserDeviceTokenService : ICrudService<UserDeviceTokenBM>
{
    Task<UserDeviceTokenBM> RegisterOrUpdateTokenAsync(int userId, string fcmToken);
    Task<bool> DeactivateTokenAsync(int userId, string fcmToken);
    Task<bool> DeactivateAllUserTokensAsync(int userId);
    Task<IReadOnlyList<UserDeviceTokenBM>> GetActiveTokensByUserIdAsync(int userId);
}


