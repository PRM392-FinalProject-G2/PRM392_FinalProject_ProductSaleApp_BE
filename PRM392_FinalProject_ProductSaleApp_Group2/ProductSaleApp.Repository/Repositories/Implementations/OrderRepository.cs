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
                .ThenInclude(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
            .Include(o => o.Payments)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == id);
    }

    public override async Task<(IReadOnlyList<Order> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Orders
            .Include(o => o.User)
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}


