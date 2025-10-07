using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class VoucherService : CrudService<Voucher, VoucherBM>, IVoucherService
{
    private readonly IMapper _mapper;

    public VoucherService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Voucher> GetRepository() => UnitOfWork.VoucherRepository;

    public async Task<PagedResult<VoucherBM>> GetPagedFilteredAsync(VoucherBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.VoucherRepository;
        var repositoryFilter = new Voucher
        {
            Voucherid = filter?.VoucherId ?? 0,
            Code = filter?.Code,
            Isactive = filter?.IsActive ?? false
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<VoucherBM>>(entities);
        var totalPages = (int)System.Math.Ceiling((double)total / pageSize);
        return new PagedResult<VoucherBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}



