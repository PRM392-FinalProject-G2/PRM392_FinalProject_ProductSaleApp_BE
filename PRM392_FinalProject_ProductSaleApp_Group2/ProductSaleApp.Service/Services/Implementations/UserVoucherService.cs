using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

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
            Orderid = filter?.OrderId,
            Isused = filter?.IsUsed ?? false  // Chỉ filter khi có giá trị cụ thể
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

    public async Task<UserVoucherBM> GetByUserIdAndOrderIdAsync(int userId, int orderId)
    {
        var repo = UnitOfWork.UserVoucherRepository;
        var entity = await repo.GetByUserIdAndOrderIdAsync(userId, orderId);
        return entity != null ? _mapper.Map<UserVoucherBM>(entity) : null;
    }

    public async Task<IReadOnlyList<UserVoucherBM>> GetActiveUnexpiredByUserIdAsync(int userId)
    {
        var repo = UnitOfWork.UserVoucherRepository;

        // Lấy tất cả voucher sở hữu bởi user, active, chưa dùng và chưa hết hạn
        var (entities, _) = await repo.GetPagedWithDetailsAsync(
            new Uservoucher
            {
                Userid = userId,
                // Isused = false sẽ được áp dụng trong repository filter logic
            },
            1,
            int.MaxValue
        );

        var now = System.DateTime.Now;
        var filtered = entities
            .Where(uv => uv.Voucher != null
                         && uv.Voucher.Isactive
                         && !uv.Isused
                         && uv.Voucher.Startdate <= now
                         && uv.Voucher.Enddate >= now)
            .ToList();

        return _mapper.Map<IReadOnlyList<UserVoucherBM>>(filtered);
    }
}



