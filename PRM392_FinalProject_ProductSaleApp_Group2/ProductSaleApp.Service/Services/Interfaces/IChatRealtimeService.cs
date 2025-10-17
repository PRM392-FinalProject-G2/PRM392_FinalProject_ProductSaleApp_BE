using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IChatRealtimeService
{
    Task<List<ConversationBM>> GetUserConversationsAsync(int userId);
    Task<List<ChatMessageBM>> GetChatHistoryAsync(int userId, int otherUserId, int pageNumber = 1, int pageSize = 50);
    Task<int> GetUnreadMessageCountAsync(int userId, int fromUserId);
    Task MarkMessagesAsReadAsync(int userId, int fromUserId);
    Task<List<int>> GetOnlineUsersAsync();
}
