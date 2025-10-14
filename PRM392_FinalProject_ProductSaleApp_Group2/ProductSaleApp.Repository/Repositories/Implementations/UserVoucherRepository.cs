using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class UserVoucherRepository : EntityRepository<Uservoucher>, IUserVoucherRepository
{
    private readonly SalesAppDBContext _dbContext;

    public UserVoucherRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Uservoucher> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Uservouchers
            .Include(uv => uv.User)
            .Include(uv => uv.Voucher)
            .Include(uv => uv.Order)
            .AsNoTracking()
            .FirstOrDefaultAsync(uv => uv.Uservoucherid == id);
    }

    public override async Task<(IReadOnlyList<Uservoucher> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Uservouchers
            .Include(uv => uv.User)
            .Include(uv => uv.Voucher)
            .Include(uv => uv.Order)
            .AsNoTracking()
            .OrderByDescending(uv => uv.Assignedat);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<(IReadOnlyList<Uservoucher> Items, int Total)> GetPagedWithDetailsAsync(Uservoucher filter, int pageNumber, int pageSize)
    {
        var query = _dbContext.Uservouchers
            .Include(uv => uv.User)
            .Include(uv => uv.Voucher)
            .Include(uv => uv.Order)
            .AsNoTracking()
            .AsQueryable();

        // Filter only active vouchers
        query = query.Where(uv => uv.Voucher.Isactive);

        if (filter != null)
        {
            if (filter.Uservoucherid > 0)
                query = query.Where(uv => uv.Uservoucherid == filter.Uservoucherid);
            if (filter.Userid > 0)
                query = query.Where(uv => uv.Userid == filter.Userid);
            if (filter.Voucherid > 0)
                query = query.Where(uv => uv.Voucherid == filter.Voucherid);
            if (filter.Isused)
                query = query.Where(uv => uv.Isused);
            if(!filter.Isused)
                query = query.Where(uv => !uv.Isused);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(uv => uv.Assignedat)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }

    public async Task<Uservoucher> GetByUserIdAndOrderIdAsync(int userId, int orderId)
    {
        return await _dbContext.Uservouchers
            .Include(uv => uv.User)
            .Include(uv => uv.Voucher)
            .Include(uv => uv.Order)
            .AsNoTracking()
            .FirstOrDefaultAsync(uv => uv.Userid == userId && uv.Orderid == orderId);
    }
}



