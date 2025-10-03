using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class WishlistRepository : EntityRepository<Wishlist>, IWishlistRepository
{
    private readonly SalesAppDBContext _dbContext;

    public WishlistRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Wishlist> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Wishlists
            .Include(w => w.Product)
            .Include(w => w.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.WishlistId == id);
    }

    public override async Task<(IReadOnlyList<Wishlist> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Wishlists
            .Include(w => w.Product)
            .Include(w => w.User)
            .AsNoTracking()
            .OrderByDescending(w => w.CreatedAt);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}



