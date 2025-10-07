using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IProductService : ICrudService<ProductBM>
{
    Task<PagedResult<ProductBM>> GetPagedFilteredAsync(ProductBM filter, int pageNumber, int pageSize);
}


