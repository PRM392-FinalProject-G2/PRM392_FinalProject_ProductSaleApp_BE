using ProductSaleApp.Repository.Models;

namespace ProductSaleApp.Repository.Repositories.Interfaces;

public interface IProductVoucherRepository : IEntityRepository<Productvoucher>
{
    Task<(IReadOnlyList<Productvoucher> Items, int Total)> GetPagedWithDetailsAsync(Productvoucher filter, int pageNumber, int pageSize);
}



