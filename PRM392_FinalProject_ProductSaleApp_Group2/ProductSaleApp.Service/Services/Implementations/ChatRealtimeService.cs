using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class ChatRealtimeService : IChatRealtimeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly IChatMessageService _chatMessageService;
    private readonly IUserService _userService;

    public ChatRealtimeService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        IMemoryCache cache,
        IChatMessageService chatMessageService,
        IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
        _chatMessageService = chatMessageService;
        _userService = userService;
    }

    public async Task<List<ConversationBM>> GetUserConversationsAsync(int userId)
    {
        // Lấy tất cả tin nhắn liên quan đến user (với pagination lớn)
        var allMessagesResult = await _chatMessageService.GetPagedFilteredAsync(new ChatMessageBM(), 1, 10000);
        var allMessages = allMessagesResult.Items.ToList();
        
        // Lấy tất cả users mà userId đã chat
        var conversationUserIds = allMessages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .SelectMany(m => new[] { m.SenderId, m.ReceiverId })
            .Where(id => id != userId)
            .Distinct()
            .ToList();

        var conversations = new List<ConversationBM>();

        foreach (var otherUserId in conversationUserIds)
        {
            var conversationMessages = allMessages
                .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                           (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderByDescending(m => m.SentAt)
                .ToList();

            if (conversationMessages.Any())
            {
                var lastMessage = conversationMessages.First();
                var otherUser = await _userService.GetByIdAsync(otherUserId);
                var unreadCount = await GetUnreadMessageCountAsync(userId, otherUserId);

                conversations.Add(new ConversationBM
                {
                    UserId = otherUserId,
                    UserName = otherUser?.Username ?? "Unknown",
                    UserAvatar = otherUser?.Avatarurl,
                    LastMessage = lastMessage.Message,
                    LastMessageTime = lastMessage.SentAt,
                    LastMessageSenderId = lastMessage.SenderId,
                    UnreadCount = unreadCount,
                    IsOnline = CheckUserOnlineStatus(otherUserId)
                });
            }
        }

        return conversations.OrderByDescending(c => c.LastMessageTime).ToList();
    }

    public async Task<List<ChatMessageBM>> GetChatHistoryAsync(int userId, int otherUserId, int pageNumber = 1, int pageSize = 50)
    {
        // Lấy tất cả tin nhắn với pagination lớn
        var allMessagesResult = await _chatMessageService.GetPagedFilteredAsync(new ChatMessageBM(), 1, 10000);
        var allMessages = allMessagesResult.Items.ToList();
        
        var messages = allMessages
            .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                       (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderByDescending(m => m.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return messages;
    }

    public async Task<int> GetUnreadMessageCountAsync(int userId, int fromUserId)
    {
        // Lấy từ cache hoặc database
        var cacheKey = $"unread_{userId}_{fromUserId}";
        
        if (_cache.TryGetValue(cacheKey, out int count))
        {
            return count;
        }

        // Tính toán unread count (cần thêm field IsRead trong database)
        // Tạm thời return 0
        return await Task.FromResult(0);
    }

    public async Task MarkMessagesAsReadAsync(int userId, int fromUserId)
    {
        // Clear cache
        var cacheKey = $"unread_{userId}_{fromUserId}";
        _cache.Remove(cacheKey);

        // Update database (cần thêm field IsRead)
        await Task.CompletedTask;
    }

    public async Task<List<int>> GetOnlineUsersAsync()
    {
        // Lấy danh sách users online từ cache
        var cacheKey = "online_users";
        
        if (_cache.TryGetValue(cacheKey, out List<int> onlineUsers))
        {
            return onlineUsers;
        }

        return await Task.FromResult(new List<int>());
    }

    private bool CheckUserOnlineStatus(int userId)
    {
        var cacheKey = $"user_online_{userId}";
        return _cache.TryGetValue(cacheKey, out bool isOnline) && isOnline;
    }
}
