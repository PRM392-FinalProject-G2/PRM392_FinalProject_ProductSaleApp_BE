using System.Collections.Generic;
using System.Threading.Tasks;
using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface ICrudService<TBusinessModel>
{
    Task<TBusinessModel> GetByIdAsync(int id, bool includeDetails = true);

    Task<PagedResult<TBusinessModel>> GetPagedAsync(int pageNumber, int pageSize);

    Task<TBusinessModel> CreateAsync(TBusinessModel model);

    Task<TBusinessModel> UpdateAsync(int id, TBusinessModel model);

    Task<bool> DeleteAsync(int id);
}


