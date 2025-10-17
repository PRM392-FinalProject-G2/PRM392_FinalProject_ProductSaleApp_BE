using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IPaymentWorkflowService
{
	Task<VnPayCallbackResult> ProcessVnPayCallbackAsync(IQueryCollection query);
	Task<CreateOrderPaymentResult> CreateOrderAndPaymentAsync(OrderBM orderRequest, int? voucherId, string clientIp);
    Task<bool> HandleSuccessfulPaymentCartManagementAsync(int orderId, decimal finalAmount);
    Task<AmountCalculationResult> CalculateOrderAmountAsync(int cartId, int? userId, int? voucherId);
    Task<CreateOrderPaymentResult> CreatePaymentWithFinalAmountAsync(OrderBM orderRequest, int? voucherId, decimal finalAmount, string clientIp);
}




