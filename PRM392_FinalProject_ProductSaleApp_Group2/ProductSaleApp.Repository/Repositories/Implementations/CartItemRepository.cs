using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class CartItemRepository : EntityRepository<CartItem>, ICartItemRepository
{
    private readonly SalesAppDBContext _dbContext;

    public CartItemRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<CartItem> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.CartItems
            .Include(ci => ci.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(ci => ci.CartItemId == id);
    }

    public override async Task<(IReadOnlyList<CartItem> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.CartItems
            .Include(ci => ci.Product)
            .AsNoTracking()
            .OrderByDescending(ci => ci.CartItemId);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}


