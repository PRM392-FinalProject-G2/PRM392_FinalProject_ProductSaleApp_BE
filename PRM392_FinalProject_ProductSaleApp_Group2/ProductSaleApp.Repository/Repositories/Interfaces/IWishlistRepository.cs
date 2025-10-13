using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IWishlistRepository : IEntityRepository<Wishlist>
{
    Task<(IReadOnlyList<Wishlist> Items, int Total)> GetPagedWithDetailsAsync(Wishlist filter, int pageNumber, int pageSize);
    Task<(IReadOnlyList<Wishlist> Items, int Total)> GetMobilePagedWithDetailsAsync(Wishlist filter, int pageNumber, int pageSize);
}



