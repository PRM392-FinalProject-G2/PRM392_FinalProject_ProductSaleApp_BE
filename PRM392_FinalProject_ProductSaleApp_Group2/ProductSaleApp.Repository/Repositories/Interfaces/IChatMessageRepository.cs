using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IChatMessageRepository : IEntityRepository<Chatmessage>
{
    Task<(IReadOnlyList<Chatmessage> Items, int Total)> GetPagedWithDetailsAsync(Chatmessage filter, int pageNumber, int pageSize);
}


