using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class UserRepository : EntityRepository<User>, IUserRepository
{
    private readonly SalesAppDBContext _dbContext;

    public UserRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<User> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Userid == id);
    }

    public override async Task<(IReadOnlyList<User> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Users
            .AsNoTracking()
            .OrderBy(u => u.Username);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<(IReadOnlyList<User> Items, int Total)> GetPagedWithDetailsAsync(User filter, int pageNumber, int pageSize)
    {
        var query = _dbContext.Users
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (filter.Userid > 0)
                query = query.Where(u => u.Userid == filter.Userid);
            if (!string.IsNullOrWhiteSpace(filter.Username))
                query = query.Where(u => u.Username.Contains(filter.Username));
            if (!string.IsNullOrWhiteSpace(filter.Email))
                query = query.Where(u => u.Email.Contains(filter.Email));
            if (!string.IsNullOrWhiteSpace(filter.Phonenumber))
                query = query.Where(u => u.Phonenumber.Contains(filter.Phonenumber));
            if (!string.IsNullOrWhiteSpace(filter.Role))
                query = query.Where(u => u.Role == filter.Role);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.Username)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }
}


