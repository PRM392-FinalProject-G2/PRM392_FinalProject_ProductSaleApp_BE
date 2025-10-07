using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IOrderService : ICrudService<OrderBM>
{
    Task<PagedResult<OrderBM>> GetPagedFilteredAsync(OrderBM filter, int pageNumber, int pageSize);
}


