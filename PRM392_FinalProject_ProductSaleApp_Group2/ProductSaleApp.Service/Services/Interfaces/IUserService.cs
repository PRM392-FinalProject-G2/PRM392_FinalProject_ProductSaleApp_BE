using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IUserService : ICrudService<UserBM>
{
    Task<PagedResult<UserBM>> GetPagedFilteredAsync(UserBM filter, int pageNumber, int pageSize);
    Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null);
    Task<bool> IsPhoneNumberExistsAsync(string phoneNumber, int? excludeUserId = null);
}


