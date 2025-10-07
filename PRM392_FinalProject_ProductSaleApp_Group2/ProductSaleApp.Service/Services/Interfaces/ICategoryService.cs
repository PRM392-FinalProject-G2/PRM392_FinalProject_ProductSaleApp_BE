using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface ICategoryService : ICrudService<CategoryBM>
{
    Task<PagedResult<CategoryBM>> GetPagedFilteredAsync(CategoryBM filter, int pageNumber, int pageSize);
}


