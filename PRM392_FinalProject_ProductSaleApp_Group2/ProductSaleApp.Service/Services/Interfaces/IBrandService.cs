using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IBrandService : ICrudService<BrandBM>
{
    Task<PagedResult<BrandBM>> GetPagedFilteredAsync(BrandBM filter, int pageNumber, int pageSize);
}



