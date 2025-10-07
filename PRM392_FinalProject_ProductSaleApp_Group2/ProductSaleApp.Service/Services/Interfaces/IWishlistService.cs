using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IWishlistService : ICrudService<WishlistBM>
{
    Task<PagedResult<WishlistBM>> GetPagedFilteredAsync(WishlistBM filter, int pageNumber, int pageSize);
}



