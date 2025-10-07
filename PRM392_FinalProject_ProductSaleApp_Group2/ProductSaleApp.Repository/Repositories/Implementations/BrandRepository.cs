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
            .FirstOrDefaultAsync(b => b.Brandid == id);
    }


    public async Task<(IReadOnlyList<Brand> Items, int Total)> GetPagedWithDetailsAsync(Brand? filter, int pageNumber, int pageSize)
    {
        var query = _dbContext.Brands.AsNoTracking().AsQueryable();

        if (filter != null)
        {
            if (filter.Brandid > 0)
                query = query.Where(c => c.Brandid == filter.Brandid);

            if (!string.IsNullOrWhiteSpace(filter.Brandname))
                query = query.Where(c => c.Brandname.ToLower().Contains(filter.Brandname.ToLower()));

            if (!string.IsNullOrWhiteSpace(filter.Description))
                query = query.Where(c => c.Description.ToLower().Contains(filter.Description.ToLower()));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Brandid)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

}



