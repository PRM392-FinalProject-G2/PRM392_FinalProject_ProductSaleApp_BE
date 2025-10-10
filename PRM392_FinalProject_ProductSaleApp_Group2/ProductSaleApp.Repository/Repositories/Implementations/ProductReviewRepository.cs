using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class ProductReviewRepository : EntityRepository<Productreview>, IProductReviewRepository
{
    private readonly SalesAppDBContext _dbContext;

    public ProductReviewRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Productreview> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Productreviews
            .Include(pr => pr.Product)
            .Include(pr => pr.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(pr => pr.Reviewid == id);
    }

    public override async Task<(IReadOnlyList<Productreview> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Productreviews
            .Include(pr => pr.Product)
            .Include(pr => pr.User)
            .AsNoTracking()
            .OrderByDescending(pr => pr.Createdat);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<IReadOnlyList<Productreview>> GetByProductIdAsync(int productId)
    {
        return await _dbContext.Productreviews
            .Include(pr => pr.User)
            .Where(pr => pr.Productid == productId)
            .OrderByDescending(pr => pr.Createdat)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Productreview>> GetByUserIdAsync(int userId)
    {
        return await _dbContext.Productreviews
            .Include(pr => pr.Product)
            .Where(pr => pr.Userid == userId)
            .OrderByDescending(pr => pr.Createdat)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<Productreview> Items, int Total)> GetPagedByProductIdAsync(int productId, int pageNumber, int pageSize)
    {
        var query = _dbContext.Productreviews
            .Include(pr => pr.User)
            .Where(pr => pr.Productid == productId)
            .OrderByDescending(pr => pr.Createdat)
            .AsNoTracking();

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}


