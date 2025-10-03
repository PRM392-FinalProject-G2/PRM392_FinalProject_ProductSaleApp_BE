using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.Repositories.Implementations;

public class ChatMessageRepository : EntityRepository<ChatMessage>, IChatMessageRepository
{
    private readonly SalesAppDBContext _dbContext;

    public ChatMessageRepository(SalesAppDBContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<ChatMessage> GetByIdWithDetailsAsync(int id)
    {
        return _dbContext.ChatMessages
            .Include(cm => cm.Sender)
            .Include(cm => cm.Receiver)
            .AsNoTracking()
            .FirstOrDefaultAsync(cm => cm.ChatMessageId == id);
    }

    public override async Task<(IReadOnlyList<ChatMessage> Items, int Total)> GetPagedWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbContext.ChatMessages
            .Include(cm => cm.Sender)
            .Include(cm => cm.Receiver)
            .AsNoTracking()
            .OrderByDescending(cm => cm.SentAt);

        var total = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }
}


