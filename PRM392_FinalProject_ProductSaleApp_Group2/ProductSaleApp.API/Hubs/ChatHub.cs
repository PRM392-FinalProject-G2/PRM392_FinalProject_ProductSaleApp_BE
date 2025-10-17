using Microsoft.AspNetCore.SignalR;
using ProductSaleApp.Service.Services.Interfaces;
using ProductSaleApp.Service.BusinessModel;
using System.Collections.Concurrent;

namespace ProductSaleApp.API.Hubs;

public class ChatHub : Hub
{
    private readonly IChatMessageService _chatMessageService;
    private readonly IUserService _userService;
    
    // Dictionary để lưu userId -> connectionId mapping
    private static readonly ConcurrentDictionary<int, string> UserConnections = new();

    public ChatHub(IChatMessageService chatMessageService, IUserService userService)
    {
        _chatMessageService = chatMessageService;
        _userService = userService;
    }

    // Khi user kết nối, đăng ký userId với connectionId
    public async Task RegisterUser(int userId)
    {
        UserConnections[userId] = Context.ConnectionId;
        await Clients.Caller.SendAsync("UserRegistered", userId);
        
        // Thông báo cho user về trạng thái online
        await NotifyUserStatus(userId, true);
    }

    // Gửi tin nhắn từ user này đến user khác
    public async Task SendMessage(int senderId, int receiverId, string message)
    {
        try
        {
            // Lưu tin nhắn vào database
            var chatMessage = new ChatMessageBM
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                SentAt = DateTime.Now // Changed from UtcNow to Now for PostgreSQL compatibility
            };

            var savedMessage = await _chatMessageService.CreateAsync(chatMessage);

            // Lấy thông tin sender để trả về đầy đủ
            var sender = await _userService.GetByIdAsync(senderId);
            var messageResponse = new
            {
                ChatMessageId = savedMessage.ChatMessageId,
                SenderId = savedMessage.SenderId,
                ReceiverId = savedMessage.ReceiverId,
                Message = savedMessage.Message,
                SentAt = savedMessage.SentAt,
                SenderName = sender?.Username ?? "Unknown",
                SenderAvatar = sender?.Avatarurl
            };

            // Gửi tin nhắn đến receiver nếu đang online
            if (UserConnections.TryGetValue(receiverId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", messageResponse);
            }

            // Confirm lại cho sender
            await Clients.Caller.SendAsync("MessageSent", messageResponse);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"Failed to send message: {ex.Message}");
        }
    }

    // Gửi thông báo "đang gõ..."
    public async Task SendTypingIndicator(int senderId, int receiverId, bool isTyping)
    {
        if (UserConnections.TryGetValue(receiverId, out var receiverConnectionId))
        {
            await Clients.Client(receiverConnectionId).SendAsync("UserTyping", senderId, isTyping);
        }
    }

    // Đánh dấu tin nhắn đã đọc
    public async Task MarkMessagesAsRead(int userId, int otherUserId)
    {
        // Có thể implement logic đánh dấu đã đọc ở đây
        if (UserConnections.TryGetValue(otherUserId, out var otherUserConnectionId))
        {
            await Clients.Client(otherUserConnectionId).SendAsync("MessagesRead", userId);
        }
    }

    // Lấy danh sách user đang online
    public async Task GetOnlineUsers()
    {
        var onlineUserIds = UserConnections.Keys.ToList();
        await Clients.Caller.SendAsync("OnlineUsers", onlineUserIds);
    }

    // Thông báo trạng thái online/offline
    private async Task NotifyUserStatus(int userId, bool isOnline)
    {
        await Clients.Others.SendAsync("UserStatusChanged", userId, isOnline);
    }

    // Khi user ngắt kết nối
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var disconnectedUserId = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        
        if (disconnectedUserId != 0)
        {
            UserConnections.TryRemove(disconnectedUserId, out _);
            await NotifyUserStatus(disconnectedUserId, false);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Lấy lịch sử chat giữa 2 users
    public async Task GetChatHistory(int userId, int otherUserId, int pageNumber = 1, int pageSize = 50)
    {
        try
        {
            var filter = new ChatMessageBM
            {
                SenderId = userId,
                ReceiverId = otherUserId
            };

            // Lấy tin nhắn từ cả 2 chiều
            var messages = await _chatMessageService.GetPagedFilteredAsync(filter, pageNumber, pageSize);
            
            await Clients.Caller.SendAsync("ChatHistory", messages);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"Failed to get chat history: {ex.Message}");
        }
    }
}
