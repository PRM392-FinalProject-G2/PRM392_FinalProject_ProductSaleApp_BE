using System.Threading.Tasks;
using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IAuthService
{
    Task<AuthBM> RegisterAsync(RegisterBM model);
    Task<AuthBM> LoginAsync(LoginBM model);
    Task<bool> LogoutAsync(int userId);
}


