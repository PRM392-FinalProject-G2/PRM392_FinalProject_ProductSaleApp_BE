using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface ICartItemRepository : IEntityRepository<Cartitem>
{
    Task<(IReadOnlyList<Cartitem> Items, int Total)> GetPagedWithDetailsAsync(Cartitem filter, int pageNumber, int pageSize);
}


