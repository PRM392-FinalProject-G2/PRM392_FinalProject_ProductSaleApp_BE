using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface ICartRepository : IEntityRepository<Cart>
{
    Task<(IReadOnlyList<Cart> Items, int Total)> GetPagedWithDetailsAsync(Cart filter, int pageNumber, int pageSize);
}


