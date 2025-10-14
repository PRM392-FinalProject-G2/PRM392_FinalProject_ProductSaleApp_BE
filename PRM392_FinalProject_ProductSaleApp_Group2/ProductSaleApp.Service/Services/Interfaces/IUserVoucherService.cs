using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IUserVoucherService : ICrudService<UserVoucherBM>
{
    Task<PagedResult<UserVoucherBM>> GetPagedFilteredAsync(UserVoucherBM filter, int pageNumber, int pageSize);
    Task<UserVoucherBM> GetByUserIdAndOrderIdAsync(int userId, int orderId);
}



