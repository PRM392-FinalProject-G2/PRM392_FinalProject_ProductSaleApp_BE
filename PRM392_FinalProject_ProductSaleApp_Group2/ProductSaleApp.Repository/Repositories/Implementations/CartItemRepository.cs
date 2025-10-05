using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class CartItemRepository : EntityRepository<Cartitem>, ICartItemRepository
{
    private readonly SalesAppDBContext _dbContext;

    public CartItemRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Cartitem> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Cartitems
            .Include(ci => ci.Product)
                .ThenInclude(p => p.Category)
            .Include(c => c.Product)
                .ThenInclude(p => p.Brand)
            .AsNoTracking()
            .FirstOrDefaultAsync(ci => ci.Cartitemid == id);
    }

    public override async Task<(IReadOnlyList<Cartitem> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Cartitems
            .Include(ci => ci.Product)
                .ThenInclude(p => p.Category)
            .Include(c => c.Product)
                .ThenInclude(p => p.Brand)
            .AsNoTracking()
            .OrderByDescending(ci => ci.Cartitemid);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}


