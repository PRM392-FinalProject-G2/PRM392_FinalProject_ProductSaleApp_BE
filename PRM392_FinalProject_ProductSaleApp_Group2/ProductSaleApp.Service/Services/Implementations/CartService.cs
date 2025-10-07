using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class CartService : CrudService<Cart, CartBM>, ICartService
{
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Cart> GetRepository() => UnitOfWork.CartRepository;

    public async Task<PagedResult<CartBM>> GetPagedFilteredAsync(CartBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.CartRepository;
        var repositoryFilter = new Cart
        {
            Cartid = filter?.CartId ?? 0,
            Userid = filter?.UserId,
            Status = filter?.Status
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<CartBM>>(entities);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);
        return new PagedResult<CartBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}


