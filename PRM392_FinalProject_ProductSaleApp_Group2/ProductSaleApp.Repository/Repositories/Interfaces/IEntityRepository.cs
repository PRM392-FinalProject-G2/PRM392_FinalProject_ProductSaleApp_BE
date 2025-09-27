using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IEntityRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdWithDetailsAsync(int id);
    Task<(IReadOnlyList<TEntity> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize);
}


