using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class OrderRepository : EntityRepository<Order>, IOrderRepository
{
    private readonly SalesAppDBContext _dbContext;

    public OrderRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Order> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Cart)
                .ThenInclude(c => c.Cartitems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
            .Include(o => o.Cart)
                .ThenInclude(c => c.Cartitems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Productimages)
            .Include(o => o.Cart)
                .ThenInclude(c => c.Cartitems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Brand)
            .Include(o => o.Payments)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Orderid == id);
    }

    public override async Task<(IReadOnlyList<Order> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Orders
            .Include(o => o.User)
            .AsNoTracking()
            .OrderByDescending(o => o.Orderdate);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<(IReadOnlyList<Order> Items, int Total)> GetPagedWithDetailsAsync(Order filter, int pageNumber, int pageSize)
    {
        var query = _dbContext.Orders
            .Include(o => o.User)
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (filter.Orderid > 0)
                query = query.Where(o => o.Orderid == filter.Orderid);
            if (filter.Userid.HasValue)
                query = query.Where(o => o.Userid == filter.Userid);
            if (filter.Cartid.HasValue)
                query = query.Where(o => o.Cartid == filter.Cartid);
            if (!string.IsNullOrWhiteSpace(filter.Orderstatus))
                query = query.Where(o => o.Orderstatus == filter.Orderstatus);
        }

        var total = await query.CountAsync();
        var items = await query
            .Include(o => o.Cart)
                .ThenInclude(c => c.Cartitems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Productimages)
            .OrderByDescending(o => o.Orderdate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }
}


