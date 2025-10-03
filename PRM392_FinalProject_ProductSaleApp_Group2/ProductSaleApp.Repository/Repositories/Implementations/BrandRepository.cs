using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class BrandRepository : EntityRepository<Brand>, IBrandRepository
{
    private readonly SalesAppDBContext _dbContext;

    public BrandRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Brand> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BrandId == id);
    }

    public override async Task<(IReadOnlyList<Brand> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Brands
            .AsNoTracking()
            .OrderBy(b => b.BrandName);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}



