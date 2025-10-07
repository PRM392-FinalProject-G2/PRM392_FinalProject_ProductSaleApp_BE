using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IVoucherRepository : IEntityRepository<Voucher>
{
    Task<(IReadOnlyList<Voucher> Items, int Total)> GetPagedWithDetailsAsync(Voucher filter, int pageNumber, int pageSize);
}



