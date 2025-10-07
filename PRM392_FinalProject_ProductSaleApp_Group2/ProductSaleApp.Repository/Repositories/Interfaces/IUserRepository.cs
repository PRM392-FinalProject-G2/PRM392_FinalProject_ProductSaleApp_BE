using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IUserRepository : IEntityRepository<User>
{
    Task<(IReadOnlyList<User> Items, int Total)> GetPagedWithDetailsAsync(User filter, int pageNumber, int pageSize);
}


