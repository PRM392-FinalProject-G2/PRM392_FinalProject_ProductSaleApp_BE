using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class UserDeviceTokenRepository : EntityRepository<Userdevicetoken>, IUserDeviceTokenRepository
{
    private readonly SalesAppDBContext _dbContext;

    public UserDeviceTokenRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Userdevicetoken> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Userdevicetokens
            .Include(t => t.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Tokenid == id);
    }

    public override async Task<(IReadOnlyList<Userdevicetoken> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Userdevicetokens
            .Include(t => t.User)
            .AsNoTracking()
            .OrderByDescending(t => t.Lastupdateddate);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<Userdevicetoken> GetByFcmTokenAsync(string fcmToken)
    {
        return await _dbContext.Userdevicetokens
            .Include(t => t.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Fcmtoken == fcmToken);
    }

    public async Task<IReadOnlyList<Userdevicetoken>> GetActiveTokensByUserIdAsync(int userId)
    {
        return await _dbContext.Userdevicetokens
            .Where(t => t.Userid == userId && t.Isactive)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task DeactivateTokensByUserIdAsync(int userId)
    {
        var tokens = await _dbContext.Userdevicetokens
            .Where(t => t.Userid == userId && t.Isactive)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Isactive = false;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Userdevicetoken> GetByUserIdAndTokenAsync(int userId, string fcmToken)
    {
        return await _dbContext.Userdevicetokens
            .FirstOrDefaultAsync(t => t.Userid == userId && t.Fcmtoken == fcmToken);
    }
}


