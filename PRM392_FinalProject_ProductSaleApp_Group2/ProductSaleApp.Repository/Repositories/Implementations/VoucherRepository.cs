using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class VoucherRepository : EntityRepository<Voucher>, IVoucherRepository
{
    private readonly SalesAppDBContext _dbContext;

    public VoucherRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Voucher> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Vouchers
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VoucherId == id);
    }

    public override async Task<(IReadOnlyList<Voucher> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Vouchers
            .AsNoTracking()
            .OrderByDescending(v => v.IsActive)
            .ThenBy(v => v.Code);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}



