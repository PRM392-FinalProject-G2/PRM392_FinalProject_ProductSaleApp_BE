using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _config = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            // Lấy cấu hình từ env hoặc appsettings
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_SETTINGS__APIKEY")
                         ?? _config["SendGrid:ApiKey"];
            var fromEmail = Environment.GetEnvironmentVariable("SENDGRID_SETTINGS__FROMEMAIL")
                            ?? _config["SendGrid:FromEmail"];
            var fromName = Environment.GetEnvironmentVariable("SENDGRID_SETTINGS__FROMNAME")
                           ?? _config["SendGrid:FromName"];

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(fromEmail))
            {
                _logger.LogError("SendGrid configuration missing or invalid.");
                return false;
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var toAddress = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, plainTextContent: null, htmlContent: body);

            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"✅ Email sent successfully to {to}");
                return true;
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError($"❌ Failed to send email to {to}. Status: {response.StatusCode}, Body: {responseBody}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception while sending email to {to}");
            return false;
        }
    }

    public async Task<bool> SendOtpEmailAsync(string toEmail, string otp)
    {
        var subject = "Password Reset OTP Code";
        var body = GenerateOtpEmailBody(otp);
        
        return await SendEmailAsync(toEmail, subject, body);
    }

    private string GenerateOtpEmailBody(string otp)
    {
        return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>Password Reset Request</h2>
                    <p>You have requested to reset your password. Please use the following OTP code:</p>
                    <div style='background-color: #f4f4f4; padding: 20px; text-align: center; margin: 20px 0;'>
                        <h1 style='color: #4CAF50; letter-spacing: 10px; margin: 0;'>{otp}</h1>
                    </div>
                    <p>This OTP code will expire in 5 minutes.</p>
                    <p>If you did not request this password reset, please ignore this email.</p>
                    <hr style='margin-top: 30px; border: none; border-top: 1px solid #ddd;'>
                    <p style='color: #666; font-size: 12px;'>This is an automated message, please do not reply.</p>
                </div>
            </body>
            </html>
        ";
    }
}

