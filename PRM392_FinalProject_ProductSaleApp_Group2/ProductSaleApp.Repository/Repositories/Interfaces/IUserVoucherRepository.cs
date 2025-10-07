using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IUserVoucherRepository : IEntityRepository<Uservoucher>
{
    Task<(IReadOnlyList<Uservoucher> Items, int Total)> GetPagedWithDetailsAsync(Uservoucher filter, int pageNumber, int pageSize);
}



