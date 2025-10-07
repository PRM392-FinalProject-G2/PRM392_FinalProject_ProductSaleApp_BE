using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IProductVoucherService : ICrudService<ProductVoucherBM>
{
    Task<PagedResult<ProductVoucherBM>> GetPagedFilteredAsync(ProductVoucherBM filter, int pageNumber, int pageSize);
}



