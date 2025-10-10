using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class ProductRepository : EntityRepository<Product>, IProductRepository
{
    private readonly SalesAppDBContext _dbContext;

    public ProductRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Product> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Productimages)
            .Include(p => p.Productreviews)
                .ThenInclude(pr => pr.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Productid == id);
    }

    public override async Task<(IReadOnlyList<Product> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Productimages)
            .Include(p => p.Productreviews)
                .ThenInclude(pr => pr.User)
            .AsNoTracking()
            .OrderByDescending(p => p.Productid);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<(IReadOnlyList<Product> Items, int Total)> GetPagedWithDetailsAsync(Product filter, int pageNumber, int pageSize)
    {
        var query = _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Productimages)
            .Include(p => p.Productreviews)
                .ThenInclude(pr => pr.User)
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (filter.Productid > 0)
                query = query.Where(p => p.Productid == filter.Productid);
            if (!string.IsNullOrWhiteSpace(filter.Productname))
                query = query.Where(p => p.Productname.Contains(filter.Productname));
            if (filter.Categoryid.HasValue)
                query = query.Where(p => p.Categoryid == filter.Categoryid);
            if (filter.Brandid.HasValue)
                query = query.Where(p => p.Brandid == filter.Brandid);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.Productid)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }
}


