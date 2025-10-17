using Microsoft.AspNetCore.Http;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IVnPayService
{
    string CreatePaymentUrl(int paymentId, int orderId, decimal amount, string clientIp, string orderInfo = null);
    bool ValidateCallback(IQueryCollection query, out string txnStatus, out string txnRef, out decimal amount, out string message);
} 