using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class ProductVoucherService : CrudService<Productvoucher, ProductVoucherBM>, IProductVoucherService
{
    private readonly IMapper _mapper;

    public ProductVoucherService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Productvoucher> GetRepository() => UnitOfWork.ProductVoucherRepository;

    public async Task<PagedResult<ProductVoucherBM>> GetPagedFilteredAsync(ProductVoucherBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.ProductVoucherRepository;
        var repositoryFilter = new Productvoucher
        {
            Productvoucherid = filter?.ProductVoucherId ?? 0,
            Productid = filter?.ProductId ?? 0,
            Voucherid = filter?.VoucherId ?? 0
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<ProductVoucherBM>>(entities);
        var totalPages = (int)System.Math.Ceiling((double)total / pageSize);
        return new PagedResult<ProductVoucherBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}



