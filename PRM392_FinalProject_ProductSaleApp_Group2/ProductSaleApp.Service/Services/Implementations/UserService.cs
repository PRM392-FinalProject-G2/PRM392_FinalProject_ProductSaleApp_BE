using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class UserService : CrudService<User, UserBM>, IUserService
{
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<User> GetRepository() => UnitOfWork.UserRepository;

    public async Task<PagedResult<UserBM>> GetPagedFilteredAsync(UserBM filter, int pageNumber, int pageSize)
    {
        var repo = UnitOfWork.UserRepository;
        var repositoryFilter = new User
        {
            Userid = filter?.UserId ?? 0,
            Username = filter?.Username,
            Email = filter?.Email,
            Phonenumber = filter?.PhoneNumber,
            Role = filter?.Role
        };

        var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);
        var items = _mapper.Map<IReadOnlyList<UserBM>>(entities);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);
        return new PagedResult<UserBM>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = total,
            TotalPages = totalPages,
            Items = items
        };
    }

    public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var repo = UnitOfWork.UserRepository;
        var count = await repo.CountAsync(u => 
            u.Email == email && 
            (!excludeUserId.HasValue || u.Userid != excludeUserId.Value));
        return count > 0;
    }

    public async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber, int? excludeUserId = null)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var repo = UnitOfWork.UserRepository;
        var count = await repo.CountAsync(u => 
            u.Phonenumber == phoneNumber && 
            (!excludeUserId.HasValue || u.Userid != excludeUserId.Value));
        return count > 0;
    }
}


