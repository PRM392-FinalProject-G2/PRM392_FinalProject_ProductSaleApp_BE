using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class WishlistService : CrudService<Wishlist, WishlistBM>, IWishlistService
{
    private readonly IMapper _mapper;

    public WishlistService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Wishlist> GetRepository() => UnitOfWork.WishlistRepository;

    public async Task<PagedResult<WishlistBM>> GetPagedFilteredAsync(WishlistBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.WishlistRepository;
        var repositoryFilter = new Wishlist
        {
            Wishlistid = filter?.WishlistId ?? 0,
            Userid = filter?.UserId ?? 0,
            Productid = filter?.ProductId ?? 0
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<WishlistBM>>(entities);
        var totalPages = (int)System.Math.Ceiling((double)total / pageSize);
        return new PagedResult<WishlistBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}



