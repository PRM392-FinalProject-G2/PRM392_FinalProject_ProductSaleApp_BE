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
    public CartItemService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Cartitem> GetRepository() => UnitOfWork.CartItemRepository;
}


