using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface ICategoryRepository : IEntityRepository<Category>
{
    Task<(IReadOnlyList<Category> Items, int Total)> GetPagedWithDetailsAsync(Category filter, int pageNumber, int pageSize);
}


