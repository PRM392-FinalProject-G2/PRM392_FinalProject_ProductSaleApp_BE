using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class StoreLocationRepository : EntityRepository<StoreLocation>, IStoreLocationRepository
{
    private readonly SalesAppDBContext _dbContext;

    public StoreLocationRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<StoreLocation> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.StoreLocations
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.LocationId == id);
    }

    public override async Task<(IReadOnlyList<StoreLocation> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.StoreLocations
            .AsNoTracking()
            .OrderBy(s => s.LocationId);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}


