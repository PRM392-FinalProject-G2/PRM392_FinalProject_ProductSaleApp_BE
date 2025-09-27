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
            .FirstOrDefaultAsync(p => p.PaymentId == id);
    }

    public override async Task<(IReadOnlyList<Payment> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Payments
            .AsNoTracking()
            .OrderByDescending(p => p.PaymentDate);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}


