using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IProductImageService : ICrudService<ProductImageBM>
{
    Task<IReadOnlyList<ProductImageBM>> GetByProductIdAsync(int productId);
}


