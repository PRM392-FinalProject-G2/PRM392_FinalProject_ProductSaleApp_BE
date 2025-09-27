using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly SalesAppDBContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(SalesAppDBContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }

    public async Task<TEntity> GetByIdAsync(object id, params Expression<Func<TEntity, object>>[] includes)
    {
        var entityType = _dbContext.Model.FindEntityType(typeof(TEntity));
        var key = entityType.FindPrimaryKey();

        if (key.Properties.Count != 1)
        {
            // Fallback: FindAsync for composite keys without includes
            return await _dbSet.FindAsync(id);
        }

        var keyName = key.Properties[0].Name;

        IQueryable<TEntity> query = _dbSet;

        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var property = Expression.Call(
            typeof(EF).GetMethod("Property").MakeGenericMethod(typeof(object)),
            parameter,
            Expression.Constant(keyName));
        var constant = Expression.Convert(Expression.Constant(id), typeof(object));
        var body = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return await query.AsNoTracking().FirstOrDefaultAsync(lambda);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        int? pageNumber = null,
        int? pageSize = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet.AsQueryable();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
        {
            query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
    {
        if (predicate == null)
        {
            return _dbSet.CountAsync();
        }

        return _dbSet.CountAsync(predicate);
    }

    public Task AddAsync(TEntity entity)
    {
        return _dbSet.AddAsync(entity).AsTask();
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
}


