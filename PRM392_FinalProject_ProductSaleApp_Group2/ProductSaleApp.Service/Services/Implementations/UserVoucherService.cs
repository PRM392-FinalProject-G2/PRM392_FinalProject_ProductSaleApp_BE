using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class UserVoucherService : CrudService<Uservoucher, UserVoucherBM>, IUserVoucherService
{
    private readonly IMapper _mapper;

    public UserVoucherService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Uservoucher> GetRepository() => UnitOfWork.UserVoucherRepository;

    public async Task<PagedResult<UserVoucherBM>> GetPagedFilteredAsync(UserVoucherBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.UserVoucherRepository;
        var repositoryFilter = new Uservoucher
        {
            Uservoucherid = filter?.UserVoucherId ?? 0,
            Userid = filter?.UserId ?? 0,
            Voucherid = filter?.VoucherId ?? 0,
            Isused = filter?.IsUsed ?? false
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<UserVoucherBM>>(entities);
        var totalPages = (int)System.Math.Ceiling((double)total / pageSize);
        return new PagedResult<UserVoucherBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}



