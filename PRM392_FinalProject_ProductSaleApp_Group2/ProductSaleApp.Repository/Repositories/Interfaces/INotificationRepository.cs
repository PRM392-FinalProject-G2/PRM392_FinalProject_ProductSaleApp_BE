using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface INotificationRepository : IEntityRepository<Notification>
{
    Task<(IReadOnlyList<Notification> Items, int Total)> GetPagedWithDetailsAsync(Notification filter, int pageNumber, int pageSize);
}


