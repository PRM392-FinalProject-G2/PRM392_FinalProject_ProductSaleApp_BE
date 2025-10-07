using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class NotificationService : CrudService<Notification, NotificationBM>, INotificationService
{
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Notification> GetRepository() => UnitOfWork.NotificationRepository;

    public async Task<PagedResult<NotificationBM>> GetPagedFilteredAsync(NotificationBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.NotificationRepository;
        var repositoryFilter = new Notification
        {
            Notificationid = filter?.NotificationId ?? 0,
            Userid = filter?.UserId,
            Isread = filter?.IsRead ?? false
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<NotificationBM>>(entities);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);
        return new PagedResult<NotificationBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }
}


