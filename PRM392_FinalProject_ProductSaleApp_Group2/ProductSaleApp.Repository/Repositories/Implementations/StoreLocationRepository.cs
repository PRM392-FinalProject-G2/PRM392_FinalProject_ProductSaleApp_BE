using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class StoreLocationRepository : EntityRepository<Storelocation>, IStoreLocationRepository
{
    private readonly SalesAppDBContext _dbContext;

    public StoreLocationRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Storelocation> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Storelocations
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Locationid == id);
    }

    public override async Task<(IReadOnlyList<Storelocation> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Storelocations
            .AsNoTracking()
            .OrderBy(s => s.Locationid);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}


