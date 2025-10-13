using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class FirebaseNotificationService : IFirebaseNotificationService
{
    private readonly ILogger<FirebaseNotificationService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _projectId;
    private static bool _isInitialized = false;
    private static readonly object _lock = new object();

    public FirebaseNotificationService(
        IConfiguration configuration,
        ILogger<FirebaseNotificationService> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _projectId = configuration["Firebase:ProjectId"];

        InitializeFirebase(configuration);
    }

    private void InitializeFirebase(IConfiguration configuration)
    {
        if (_isInitialized) return;

        lock (_lock)
        {
            if (_isInitialized) return;

            try
            {
                var serviceAccountKeyPath = configuration["Firebase:ServiceAccountKey"];
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), serviceAccountKeyPath);

                if (!File.Exists(fullPath))
                {
                    _logger.LogError($"Firebase service account key not found at: {fullPath}");
                    return;
                }

                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(fullPath),
                        ProjectId = _projectId
                    });

                    _logger.LogInformation("Firebase Admin SDK initialized successfully");
                }

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Firebase Admin SDK");
            }
        }
    }

    public async Task<bool> SendNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string> data = null)
    {
        if (!_isInitialized || string.IsNullOrWhiteSpace(fcmToken))
        {
            _logger.LogWarning("Firebase not initialized or invalid FCM token");
            return false;
        }

        try
        {
            var message = new Message()
            {
                Token = fcmToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = data,
                Android = new AndroidConfig()
                {
                    Notification = new AndroidNotification()
                    {
                        Icon = "ic_notification_icon",
                        Color = "#4CAF50",
                        Sound = "default",
                        ChannelId = "default_channel"
                    },
                    Priority = Priority.High
                }
            };

            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation($"Successfully sent notification to {fcmToken}: {response}");
            return true;
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogError(ex, $"Firebase messaging error: {ex.MessagingErrorCode}");
            
            // If token is invalid, we might want to deactivate it
            if (ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
            {
                await DeactivateInvalidTokenAsync(fcmToken);
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification");
            return false;
        }
    }

    public async Task<bool> SendNotificationToMultipleDevicesAsync(List<string> fcmTokens, string title, string body, Dictionary<string, string> data = null)
    {
        if (!_isInitialized || fcmTokens == null || !fcmTokens.Any())
        {
            _logger.LogWarning("Firebase not initialized or no FCM tokens provided");
            return false;
        }

        try
        {
            var message = new MulticastMessage()
            {
                Tokens = fcmTokens,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = data,
                Android = new AndroidConfig()
                {
                    Notification = new AndroidNotification()
                    {
                        Icon = "ic_notification_icon",
                        Color = "#4CAF50",
                        Sound = "default",
                        ChannelId = "default_channel"
                    },
                    Priority = Priority.High
                }
            };

            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
            _logger.LogInformation($"Successfully sent {response.SuccessCount} notifications out of {fcmTokens.Count}");

            // Handle failed tokens
            if (response.FailureCount > 0)
            {
                var failedTokens = new List<string>();
                for (int i = 0; i < response.Responses.Count; i++)
                {
                    if (!response.Responses[i].IsSuccess)
                    {
                        failedTokens.Add(fcmTokens[i]);
                        _logger.LogWarning($"Failed to send to token {fcmTokens[i]}: {response.Responses[i].Exception?.Message}");
                    }
                }
            }

            return response.SuccessCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send multicast notification");
            return false;
        }
    }

    public async Task<bool> SendCartUpdateNotificationAsync(int userId, int cartItemCount)
    {
        try
        {
            var tokens = await _unitOfWork.UserDeviceTokenRepository.GetActiveTokensByUserIdAsync(userId);
            
            if (tokens == null || !tokens.Any())
            {
                _logger.LogWarning($"No active tokens found for user {userId}");
                return false;
            }

            var title = "Giỏ hàng đã cập nhật";
            var body = cartItemCount > 0 
                ? $"Bạn có {cartItemCount} sản phẩm trong giỏ hàng" 
                : "Giỏ hàng của bạn đang trống";

            var data = new Dictionary<string, string>
            {
                { "type", "cart_update" },
                { "userId", userId.ToString() },
                { "cartItemCount", cartItemCount.ToString() },
                { "badge", cartItemCount.ToString() }
            };

            var fcmTokens = tokens.Select(t => t.Fcmtoken).ToList();
            return await SendNotificationToMultipleDevicesAsync(fcmTokens, title, body, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send cart update notification to user {userId}");
            return false;
        }
    }

    public async Task<bool> SendOrderNotificationAsync(int userId, string orderStatus, string orderDetails)
    {
        try
        {
            var tokens = await _unitOfWork.UserDeviceTokenRepository.GetActiveTokensByUserIdAsync(userId);
            
            if (tokens == null || !tokens.Any())
            {
                _logger.LogWarning($"No active tokens found for user {userId}");
                return false;
            }

            var title = GetOrderNotificationTitle(orderStatus);
            var body = orderDetails;

            var data = new Dictionary<string, string>
            {
                { "type", "order_update" },
                { "userId", userId.ToString() },
                { "orderStatus", orderStatus }
            };

            var fcmTokens = tokens.Select(t => t.Fcmtoken).ToList();
            return await SendNotificationToMultipleDevicesAsync(fcmTokens, title, body, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send order notification to user {userId}");
            return false;
        }
    }

    private string GetOrderNotificationTitle(string orderStatus)
    {
        return orderStatus?.ToLower() switch
        {
            "pending" => "Đơn hàng đang chờ xử lý",
            "confirmed" => "Đơn hàng đã được xác nhận",
            "shipped" => "Đơn hàng đang được giao",
            "delivered" => "Đơn hàng đã giao thành công",
            "cancelled" => "Đơn hàng đã bị hủy",
            _ => "Cập nhật đơn hàng"
        };
    }

    private async Task DeactivateInvalidTokenAsync(string fcmToken)
    {
        try
        {
            var token = await _unitOfWork.UserDeviceTokenRepository.GetByFcmTokenAsync(fcmToken);
            if (token != null)
            {
                token.Isactive = false;
                _unitOfWork.UserDeviceTokenRepository.Update(token);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Deactivated invalid FCM token: {fcmToken}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to deactivate token {fcmToken}");
        }
    }
}


