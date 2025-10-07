using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class ProductVoucherRepository : EntityRepository<Productvoucher>, IProductVoucherRepository
{
    private readonly SalesAppDBContext _dbContext;

    public ProductVoucherRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Productvoucher> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Productvouchers
            .Include(pv => pv.Product)
            .Include(pv => pv.Voucher)
            .AsNoTracking()
            .FirstOrDefaultAsync(pv => pv.Productvoucherid == id);
    }

    public override async Task<(IReadOnlyList<Productvoucher> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Productvouchers
            .Include(pv => pv.Product)
            .Include(pv => pv.Voucher)
            .AsNoTracking()
            .OrderByDescending(pv => pv.Productvoucherid);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<(IReadOnlyList<Productvoucher> Items, int Total)> GetPagedWithDetailsAsync(Productvoucher filter, int pageNumber, int pageSize)
    {
        var query = _dbContext.Productvouchers
            .Include(pv => pv.Product)
            .Include(pv => pv.Voucher)
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (filter.Productvoucherid > 0)
                query = query.Where(pv => pv.Productvoucherid == filter.Productvoucherid);
            if (filter.Productid > 0)
                query = query.Where(pv => pv.Productid == filter.Productid);
            if (filter.Voucherid > 0)
                query = query.Where(pv => pv.Voucherid == filter.Voucherid);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(pv => pv.Productvoucherid)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }
}



