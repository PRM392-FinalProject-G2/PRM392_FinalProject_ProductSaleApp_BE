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
            .Include(c => c.Cartitems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Category)
            .Include(p => p.Cartitems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Brand)
            .Include(c => c.Cartitems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Productimages)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Cartid == id);
    }

    public override async Task<(IReadOnlyList<Cart> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Carts
            .Include(c => c.User)
            .AsNoTracking()
            .OrderByDescending(c => c.Cartid);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<(IReadOnlyList<Cart> Items, int Total)> GetPagedWithDetailsAsync(Cart filter, int pageNumber, int pageSize)
    {
        var query = _dbContext.Carts
            .Include(c => c.User)
            .Include(c => c.Cartitems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Category)
            .Include(c => c.Cartitems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Brand)
            .Include(c => c.Cartitems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Productimages)
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (filter.Cartid > 0)
                query = query.Where(c => c.Cartid == filter.Cartid);
            if (filter.Userid.HasValue)
                query = query.Where(c => c.Userid == filter.Userid);
            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(c => c.Status == filter.Status);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.Cartid)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }
}


