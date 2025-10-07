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
            .FirstOrDefaultAsync(v => v.Voucherid == id);
    }

    public override async Task<(IReadOnlyList<Voucher> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Vouchers
            .AsNoTracking()
            .OrderByDescending(v => v.Isactive)
            .ThenBy(v => v.Code);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<(IReadOnlyList<Voucher> Items, int Total)> GetPagedWithDetailsAsync(Voucher filter, int pageNumber, int pageSize)
    {
        var query = _dbContext.Vouchers
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (filter.Voucherid > 0)
                query = query.Where(v => v.Voucherid == filter.Voucherid);
            if (!string.IsNullOrWhiteSpace(filter.Code))
                query = query.Where(v => v.Code.Contains(filter.Code));
            // filter.Isactive default is false; only filter when explicitly set true or explicitly false with a flag
            query = query.Where(v => !filter.Isactive || v.Isactive == filter.Isactive);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(v => v.Isactive)
            .ThenBy(v => v.Code)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }
}



