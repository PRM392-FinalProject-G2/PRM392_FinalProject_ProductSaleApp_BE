using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class ChatMessageRepository : EntityRepository<Chatmessage>, IChatMessageRepository
{
    private readonly SalesAppDBContext _dbContext;

    public ChatMessageRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<Chatmessage> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.Chatmessages
            .Include(cm => cm.Sender)
            .Include(cm => cm.Receiver)
            .AsNoTracking()
            .FirstOrDefaultAsync(cm => cm.Chatmessageid == id);
    }

    public override async Task<(IReadOnlyList<Chatmessage> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.Chatmessages
            .Include(cm => cm.Sender)
            .Include(cm => cm.Receiver)
            .AsNoTracking()
            .OrderByDescending(cm => cm.Sentat);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}


