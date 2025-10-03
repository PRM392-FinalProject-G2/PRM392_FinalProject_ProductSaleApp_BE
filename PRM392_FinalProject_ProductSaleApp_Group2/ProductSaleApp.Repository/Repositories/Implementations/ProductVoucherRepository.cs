using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class ProductVoucherRepository : EntityRepository<ProductVoucher>, IProductVoucherRepository
{
    private readonly SalesAppDBContext _dbContext;

    public ProductVoucherRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<ProductVoucher> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.ProductVouchers
            .Include(pv => pv.Product)
            .Include(pv => pv.Voucher)
            .AsNoTracking()
            .FirstOrDefaultAsync(pv => pv.ProductVoucherId == id);
    }

    public override async Task<(IReadOnlyList<ProductVoucher> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.ProductVouchers
            .Include(pv => pv.Product)
            .Include(pv => pv.Voucher)
            .AsNoTracking()
            .OrderByDescending(pv => pv.ProductVoucherId);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}



