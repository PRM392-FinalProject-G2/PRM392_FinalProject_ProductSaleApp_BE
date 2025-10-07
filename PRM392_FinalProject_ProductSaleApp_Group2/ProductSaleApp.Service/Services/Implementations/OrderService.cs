using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class OrderService : CrudService<Order, OrderBM>, IOrderService
{
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Order> GetRepository() => UnitOfWork.OrderRepository;

    public async Task<PagedResult<OrderBM>> GetPagedFilteredAsync(OrderBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.OrderRepository;
        var repositoryFilter = new Order
        {
            Orderid = filter?.OrderId ?? 0,
            Userid = filter?.UserId,
            Cartid = filter?.CartId,
            Orderstatus = filter?.OrderStatus
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<OrderBM>>(entities);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);
        return new PagedResult<OrderBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}


