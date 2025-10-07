using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IPaymentRepository : IEntityRepository<Payment>
{
    Task<(IReadOnlyList<Payment> Items, int Total)> GetPagedWithDetailsAsync(Payment filter, int pageNumber, int pageSize);
}


