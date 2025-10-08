using System.Threading.Tasks;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
    Task<bool> SendOtpEmailAsync(string toEmail, string otp);
}

