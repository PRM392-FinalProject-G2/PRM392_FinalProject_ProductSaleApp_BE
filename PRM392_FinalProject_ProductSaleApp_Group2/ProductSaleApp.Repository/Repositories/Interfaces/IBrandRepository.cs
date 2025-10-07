using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IBrandRepository : IEntityRepository<Brand>
{
    Task<(IReadOnlyList<Brand> Items, int Total)> GetPagedWithDetailsAsync(Brand? filter, int pageNumber, int pageSize);
}



