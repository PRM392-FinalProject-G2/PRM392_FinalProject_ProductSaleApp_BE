using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IProductRepository : IEntityRepository<Product>
{
    Task<(IReadOnlyList<Product> Items, int Total)> GetPagedWithDetailsAsync(Product filter, int pageNumber, int pageSize);
}


