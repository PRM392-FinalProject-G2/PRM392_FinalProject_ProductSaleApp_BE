using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface INotificationService : ICrudService<NotificationBM>
{
    Task<PagedResult<NotificationBM>> GetPagedFilteredAsync(NotificationBM filter, int pageNumber, int pageSize);
}


