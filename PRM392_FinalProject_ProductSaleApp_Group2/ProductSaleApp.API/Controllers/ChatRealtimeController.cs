using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProductSaleApp.API.Hubs;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.Services.Interfaces;
using ProductSaleApp.Service.BusinessModel;
using AutoMapper;

namespace ProductSaleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatRealtimeController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IChatMessageService _chatMessageService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public ChatRealtimeController(
        IHubContext<ChatHub> hubContext,
        IChatMessageService chatMessageService,
        IUserService userService,
        IMapper mapper)
    {
        _hubContext = hubContext;
        _chatMessageService = chatMessageService;
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    /// Lấy lịch sử chat giữa 2 users (REST API fallback)
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<PagedResponse<ChatHistoryResponse>>> GetChatHistory(
        [FromQuery] int userId,
        [FromQuery] int otherUserId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            // Lấy tất cả tin nhắn giữa 2 users (cả 2 chiều)
            var allMessagesResult = await _chatMessageService.GetPagedFilteredAsync(new ChatMessageBM(), 1, 10000);
            var allMessages = allMessagesResult.Items.ToList();
            
            var messages = allMessages
                .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                           (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderByDescending(m => m.SentAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = allMessages.Count(m => 
                (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                (m.SenderId == otherUserId && m.ReceiverId == userId));

            var response = new List<ChatHistoryResponse>();
            
            foreach (var msg in messages)
            {
                var sender = await _userService.GetByIdAsync(msg.SenderId);
                var receiver = await _userService.GetByIdAsync(msg.ReceiverId);
                
                response.Add(new ChatHistoryResponse
                {
                    ChatMessageId = msg.ChatMessageId,
                    SenderId = msg.SenderId,
                    ReceiverId = msg.ReceiverId,
                    Message = msg.Message,
                    SentAt = msg.SentAt,
                    SenderName = sender?.Username ?? "Unknown",
                    SenderAvatar = sender?.Avatarurl,
                    ReceiverName = receiver?.Username ?? "Unknown",
                    ReceiverAvatar = receiver?.Avatarurl
                });
            }

            return Ok(new PagedResponse<ChatHistoryResponse>
            {
                Items = response,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error getting chat history: {ex.Message}" });
        }
    }

    /// <summary>
    /// Lấy danh sách các cuộc hội thoại của user
    /// </summary>
    [HttpGet("conversations/{userId}")]
    public async Task<ActionResult<List<ConversationResponse>>> GetConversations(int userId)
    {
        try
        {
            var allMessagesResult = await _chatMessageService.GetPagedFilteredAsync(new ChatMessageBM(), 1, 10000);
            var allMessages = allMessagesResult.Items.ToList();
            
            // Lấy tất cả users mà userId đã chat
            var conversationUserIds = allMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .SelectMany(m => new[] { m.SenderId, m.ReceiverId })
                .Where(id => id != userId)
                .Distinct()
                .ToList();

            var conversations = new List<ConversationResponse>();

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

                    conversations.Add(new ConversationResponse
                    {
                        UserId = otherUserId,
                        UserName = otherUser?.Username ?? "Unknown",
                        UserAvatar = otherUser?.Avatarurl,
                        LastMessage = lastMessage.Message,
                        LastMessageTime = lastMessage.SentAt,
                        UnreadCount = 0, // Có thể implement sau
                        IsOnline = false // Có thể kiểm tra từ SignalR connections
                    });
                }
            }

            return Ok(conversations.OrderByDescending(c => c.LastMessageTime).ToList());
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error getting conversations: {ex.Message}" });
        }
    }

    /// <summary>
    /// Gửi tin nhắn (REST API fallback - nên dùng SignalR)
    /// </summary>
    [HttpPost("send")]
    public async Task<ActionResult<MessageSentResponse>> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var chatMessage = new ChatMessageBM
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Message = request.Message,
                SentAt = DateTime.Now // Changed from UtcNow to Now for PostgreSQL compatibility
            };

            var savedMessage = await _chatMessageService.CreateAsync(chatMessage);

            // Gửi thông báo qua SignalR (nếu có)
            await _hubContext.Clients.User(request.ReceiverId.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    savedMessage.ChatMessageId,
                    savedMessage.SenderId,
                    savedMessage.ReceiverId,
                    savedMessage.Message,
                    savedMessage.SentAt
                });

            return Ok(new MessageSentResponse
            {
                Success = true,
                ChatMessageId = savedMessage.ChatMessageId,
                Message = "Message sent successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new MessageSentResponse
            {
                Success = false,
                Message = $"Failed to send message: {ex.Message}"
            });
        }
    }

}
