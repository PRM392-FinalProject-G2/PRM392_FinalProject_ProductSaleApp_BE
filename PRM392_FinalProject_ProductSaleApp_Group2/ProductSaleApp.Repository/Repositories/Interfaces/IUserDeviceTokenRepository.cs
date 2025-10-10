using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IUserDeviceTokenRepository : IEntityRepository<Userdevicetoken>
{
    Task<Userdevicetoken> GetByFcmTokenAsync(string fcmToken);
    Task<IReadOnlyList<Userdevicetoken>> GetActiveTokensByUserIdAsync(int userId);
    Task DeactivateTokensByUserIdAsync(int userId);
    Task<Userdevicetoken> GetByUserIdAndTokenAsync(int userId, string fcmToken);
}


