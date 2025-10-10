using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IProductReviewRepository : IEntityRepository<Productreview>
{
    Task<IReadOnlyList<Productreview>> GetByProductIdAsync(int productId);
    Task<IReadOnlyList<Productreview>> GetByUserIdAsync(int userId);
    Task<(IReadOnlyList<Productreview> Items, int Total)> GetPagedByProductIdAsync(int productId, int pageNumber, int pageSize);
}


