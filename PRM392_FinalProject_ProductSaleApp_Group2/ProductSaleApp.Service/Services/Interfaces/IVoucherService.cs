using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IVoucherService : ICrudService<VoucherBM>
{
    Task<PagedResult<VoucherBM>> GetPagedFilteredAsync(VoucherBM filter, int pageNumber, int pageSize);
}



