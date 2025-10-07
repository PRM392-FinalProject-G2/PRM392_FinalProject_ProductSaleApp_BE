using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface ICartItemService : ICrudService<CartItemBM>
{
    Task<PagedResult<CartItemBM>> GetPagedFilteredAsync(CartItemBM filter, int pageNumber, int pageSize);
}


