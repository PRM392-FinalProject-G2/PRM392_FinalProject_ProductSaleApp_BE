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
    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Notification> GetRepository() => UnitOfWork.NotificationRepository;
}


