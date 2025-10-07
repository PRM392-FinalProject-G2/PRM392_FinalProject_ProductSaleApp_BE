using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class PaymentService : CrudService<Payment, PaymentBM>, IPaymentService
{
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Payment> GetRepository() => UnitOfWork.PaymentRepository;

    public async Task<PagedResult<PaymentBM>> GetPagedFilteredAsync(PaymentBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.PaymentRepository;
        var repositoryFilter = new Payment
        {
            Paymentid = filter?.PaymentId ?? 0,
            Orderid = filter?.OrderId,
            Paymentstatus = filter?.PaymentStatus
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<PaymentBM>>(entities);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);
        return new PagedResult<PaymentBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}


