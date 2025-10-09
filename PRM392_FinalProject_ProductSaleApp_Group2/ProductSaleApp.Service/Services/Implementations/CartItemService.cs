using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class CartItemService : CrudService<Cartitem, CartItemBM>, ICartItemService
{
    private readonly IMapper _mapper;

    public CartItemService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Cartitem> GetRepository() => UnitOfWork.CartItemRepository;

    public async Task<PagedResult<CartItemBM>> GetPagedFilteredAsync(CartItemBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.CartItemRepository;
        var repositoryFilter = new Cartitem
        {
            Cartitemid = filter?.CartItemId ?? 0,
            Cartid = filter?.CartId,
            Productid = filter?.ProductId
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<CartItemBM>>(entities);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);
        return new PagedResult<CartItemBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }

    public override async Task<CartItemBM> UpdateAsync(int id, CartItemBM model)
    {
        var repo = UnitOfWork.CartItemRepository;
        var existing = await repo.GetByIdAsync(id);

        if (existing == null)
            return default;

        existing.Quantity = model.Quantity;
        existing.Price = model.Price;
        existing.Productid = model.ProductId;
        existing.Cartid = model.CartId;

        repo.Update(existing);
        await UnitOfWork.SaveChangesAsync();

        return _mapper.Map<CartItemBM>(existing);
    }

}


