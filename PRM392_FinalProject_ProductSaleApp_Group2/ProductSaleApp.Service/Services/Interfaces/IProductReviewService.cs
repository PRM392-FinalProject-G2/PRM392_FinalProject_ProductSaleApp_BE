using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IProductReviewService : ICrudService<ProductReviewBM>
{
    Task<IReadOnlyList<ProductReviewBM>> GetByProductIdAsync(int productId);
    Task<IReadOnlyList<ProductReviewBM>> GetByUserIdAsync(int userId);
    Task<PagedResult<ProductReviewBM>> GetPagedByProductIdAsync(int productId, int pageNumber, int pageSize);
}


