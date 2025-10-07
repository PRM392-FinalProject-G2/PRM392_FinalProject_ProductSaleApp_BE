using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class CategoryRepository : EntityRepository<Category>, ICategoryRepository
{
    private readonly SalesAppDBContext _dbContext;

    public CategoryRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Category> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Categoryid == id);
    }

    public override async Task<(IReadOnlyList<Category> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Categoryname);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<(IReadOnlyList<Category> Items, int Total)> GetPagedWithDetailsAsync(Category filter, int pageNumber, int pageSize)
    {
        var query = _dbContext.Categories
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (filter.Categoryid > 0)
                query = query.Where(c => c.Categoryid == filter.Categoryid);
            if (!string.IsNullOrWhiteSpace(filter.Categoryname))
                query = query.Where(c => c.Categoryname.Contains(filter.Categoryname));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Categoryname)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }
}


