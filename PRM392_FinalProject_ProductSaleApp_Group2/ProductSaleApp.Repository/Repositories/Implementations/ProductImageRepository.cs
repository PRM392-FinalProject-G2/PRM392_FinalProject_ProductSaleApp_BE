using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class ProductImageRepository : EntityRepository<Productimage>, IProductImageRepository
{
    private readonly SalesAppDBContext _dbContext;

    public ProductImageRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Productimage> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Productimages
            .Include(pi => pi.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(pi => pi.Imageid == id);
    }

    public override async Task<(IReadOnlyList<Productimage> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Productimages
            .Include(pi => pi.Product)
            .AsNoTracking()
            .OrderByDescending(pi => pi.Imageid);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<IReadOnlyList<Productimage>> GetByProductIdAsync(int productId)
    {
        return await _dbContext.Productimages
            .Where(pi => pi.Productid == productId)
            .OrderByDescending(pi => pi.Isprimary)
            .ThenBy(pi => pi.Imageid)
            .AsNoTracking()
            .ToListAsync();
    }
}


