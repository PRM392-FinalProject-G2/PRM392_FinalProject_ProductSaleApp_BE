using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class PaymentRepository : EntityRepository<Payment>, IPaymentRepository
{
    private readonly SalesAppDBContext _dbContext;

    public PaymentRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Payment> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Paymentid == id);
    }

    public override async Task<(IReadOnlyList<Payment> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Payments
            .AsNoTracking()
            .OrderByDescending(p => p.Paymentdate);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<(IReadOnlyList<Payment> Items, int Total)> GetPagedWithDetailsAsync(Payment filter, int pageNumber, int pageSize)
    {
        var query = _dbContext.Payments
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (filter.Paymentid > 0)
                query = query.Where(p => p.Paymentid == filter.Paymentid);
            if (filter.Orderid.HasValue)
                query = query.Where(p => p.Orderid == filter.Orderid);
            if (!string.IsNullOrWhiteSpace(filter.Paymentstatus))
                query = query.Where(p => p.Paymentstatus == filter.Paymentstatus);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.Paymentdate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }
}


