using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface ICartService : ICrudService<CartBM>
{
    Task<PagedResult<CartBM>> GetPagedFilteredAsync(CartBM filter, int pageNumber, int pageSize);
    Task<bool> UpdateCartStatusAsync(int cartId, string status);
}


