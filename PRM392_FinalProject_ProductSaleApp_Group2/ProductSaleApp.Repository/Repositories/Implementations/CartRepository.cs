using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class CartRepository : EntityRepository<Cart>, ICartRepository
{
    private readonly SalesAppDBContext _dbContext;

    public CartRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Cart> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Carts
            .Include(c => c.User)
            .Include(c => c.CartItems)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CartId == id);
    }

    public override async Task<(IReadOnlyList<Cart> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Carts
            .Include(c => c.User)
            .AsNoTracking()
            .OrderByDescending(c => c.CartId);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}


