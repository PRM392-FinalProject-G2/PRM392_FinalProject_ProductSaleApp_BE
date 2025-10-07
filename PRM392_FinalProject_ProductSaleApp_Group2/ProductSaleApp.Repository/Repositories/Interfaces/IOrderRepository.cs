using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IOrderRepository : IEntityRepository<Order>
{
    Task<(IReadOnlyList<Order> Items, int Total)> GetPagedWithDetailsAsync(Order filter, int pageNumber, int pageSize);
}


