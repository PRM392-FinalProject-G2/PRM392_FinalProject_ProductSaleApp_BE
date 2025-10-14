using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IPaymentWorkflowService
{
	Task<VnPayCallbackResult> ProcessVnPayCallbackAsync(IQueryCollection query);
	Task<CreateOrderPaymentResult> CreateOrderAndPaymentAsync(OrderBM orderRequest, int? voucherId, string clientIp);
	Task<bool> HandleSuccessfulPaymentCartManagementAsync(int orderId);
}




