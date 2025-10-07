using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Services.Interfaces;

public interface IPaymentService : ICrudService<PaymentBM>
{
    Task<PagedResult<PaymentBM>> GetPagedFilteredAsync(PaymentBM filter, int pageNumber, int pageSize);
}


