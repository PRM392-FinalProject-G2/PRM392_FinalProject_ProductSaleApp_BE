using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class NotificationRepository : EntityRepository<Notification>, INotificationRepository
{
    private readonly SalesAppDBContext _dbContext;

    public NotificationRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Notification> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Notifications
            .Include(n => n.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Notificationid == id);
    }

    public override async Task<(IReadOnlyList<Notification> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Notifications
            .Include(n => n.User)
            .AsNoTracking()
            .OrderByDescending(n => n.Createdat);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<(IReadOnlyList<Notification> Items, int Total)> GetPagedWithDetailsAsync(Notification filter, int pageNumber, int pageSize)
    {
        var query = _dbContext.Notifications
            .Include(n => n.User)
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (filter.Notificationid > 0)
                query = query.Where(n => n.Notificationid == filter.Notificationid);
            if (filter.Userid.HasValue)
                query = query.Where(n => n.Userid == filter.Userid);
            if (filter.Isread.HasValue)
                query = query.Where(n => n.Isread == filter.Isread);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(n => n.Createdat)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }
}


