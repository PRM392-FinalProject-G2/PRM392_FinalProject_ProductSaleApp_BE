using System.Collections.Generic;
using System.Threading.Tasks;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class EntityRepository<TEntity> : GenericRepository<TEntity>, IEntityRepository<TEntity> where TEntity : class
{
    public EntityRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
    }

    public virtual Task<TEntity> GetByIdWithDetailsAsync(int id)
    {
        // Default: no includes; derived repos override to add Includes
        return GetByIdAsync(id);
    }

    public virtual async Task<(IReadOnlyList<TEntity> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var total = await CountAsync();
        var items = await GetAllAsync(pageNumber: pageNumber, pageSize: pageSize);
        return (items, total);
    }
}


