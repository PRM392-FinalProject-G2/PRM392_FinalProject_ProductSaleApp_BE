using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IProductImageRepository : IEntityRepository<Productimage>
{
    Task<IReadOnlyList<Productimage>> GetByProductIdAsync(int productId);
}


