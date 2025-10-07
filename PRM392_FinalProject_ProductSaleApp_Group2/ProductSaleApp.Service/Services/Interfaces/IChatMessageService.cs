using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IChatMessageService : ICrudService<ChatMessageBM>
{
    Task<PagedResult<ChatMessageBM>> GetPagedFilteredAsync(ChatMessageBM filter, int pageNumber, int pageSize);
}


