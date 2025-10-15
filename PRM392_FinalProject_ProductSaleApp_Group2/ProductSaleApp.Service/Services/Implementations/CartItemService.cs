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

        // Update Cart TotalPrice
        if (existing.Cartid.HasValue)
        {
            await UpdateCartTotalPriceAsync(existing.Cartid.Value);
        }

        return _mapper.Map<CartItemBM>(existing);
    }

    public async Task<CartItemBM> CreateCartItemAsync(CartItemBM model)
    {
        var result = await base.CreateAsync(model);
        
        // Update Cart TotalPrice
        if (model.CartId.HasValue)
        {
            await UpdateCartTotalPriceAsync(model.CartId.Value);
        }
        
        return result;
    }

    public async Task<bool> DeleteCartItemAsync(int id)
    {
        // Get CartId before deletion
        var existing = await UnitOfWork.CartItemRepository.GetByIdAsync(id);
        var cartId = existing?.Cartid;
        
        var result = await base.DeleteAsync(id);
        
        // Update Cart TotalPrice after deletion
        if (result && cartId.HasValue)
        {
            await UpdateCartTotalPriceAsync(cartId.Value);
        }
        
        return result;
    }

    private async Task UpdateCartTotalPriceAsync(int cartId)
    {
        try
        {
            // Get all CartItems for this Cart
            var cartItems = await UnitOfWork.CartItemRepository.GetPagedWithDetailsAsync(
                new Cartitem { Cartid = cartId }, 1, 1000);
            
            // Calculate total price
            decimal totalPrice = cartItems.Items.Sum(item => item.Price * item.Quantity);
            
            // Update Cart TotalPrice
            var cart = await UnitOfWork.CartRepository.GetByIdAsync(cartId);
            if (cart != null)
            {
                cart.Totalprice = totalPrice;
                UnitOfWork.CartRepository.Update(cart);
                await UnitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw to avoid breaking the main operation
            Console.WriteLine($"Error updating cart total price: {ex.Message}");
        }
    }


}


