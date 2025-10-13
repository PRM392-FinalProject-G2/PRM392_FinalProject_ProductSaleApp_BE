using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Repository.UnitOfWork;
using ProductSaleApp.Service.BusinessModel;
using ProductSaleApp.Service.Services.Interfaces;

namespace ProductSaleApp.Service.Services.Implementations;

public class UserDeviceTokenService : CrudService<Userdevicetoken, UserDeviceTokenBM>, IUserDeviceTokenService
{
    private readonly IMapper _mapper;

    public UserDeviceTokenService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
        _mapper = mapper;
    }

    protected override ProductSaleApp.Repository.Repositories.Interfaces.IEntityRepository<Userdevicetoken> GetRepository() 
        => UnitOfWork.UserDeviceTokenRepository;

    public async Task<UserDeviceTokenBM> RegisterOrUpdateTokenAsync(int userId, string fcmToken)
    {
        if (string.IsNullOrWhiteSpace(fcmToken))
            throw new ArgumentException("FCM token cannot be null or empty", nameof(fcmToken));

        var repo = UnitOfWork.UserDeviceTokenRepository;

        // First, check if this exact token exists for this user
        var existingToken = await repo.GetByUserIdAndTokenAsync(userId, fcmToken);

        if (existingToken != null)
        {
            // Update existing token
            existingToken.Isactive = true;
            existingToken.Lastupdateddate = DateTime.Now;
            repo.Update(existingToken);
            await UnitOfWork.SaveChangesAsync();
            
            return _mapper.Map<UserDeviceTokenBM>(existingToken);
        }

        // Check if token exists for different user (due to unique constraint)
        var tokenForOtherUser = await repo.GetByFcmTokenAsync(fcmToken);
        if (tokenForOtherUser != null)
        {
            // Update to new user
            tokenForOtherUser.Userid = userId;
            tokenForOtherUser.Isactive = true;
            tokenForOtherUser.Lastupdateddate = DateTime.Now;
            repo.Update(tokenForOtherUser);
            await UnitOfWork.SaveChangesAsync();
            
            return _mapper.Map<UserDeviceTokenBM>(tokenForOtherUser);
        }

        // Create new token
        var newToken = new Userdevicetoken
        {
            Userid = userId,
            Fcmtoken = fcmToken,
            Isactive = true,
            Lastupdateddate = DateTime.Now
        };

        await repo.AddAsync(newToken);
        await UnitOfWork.SaveChangesAsync();
        
        return _mapper.Map<UserDeviceTokenBM>(newToken);
    }

    public async Task<bool> DeactivateTokenAsync(int userId, string fcmToken)
    {
        if (string.IsNullOrWhiteSpace(fcmToken))
            return false;

        var repo = UnitOfWork.UserDeviceTokenRepository;
        var token = await repo.GetByUserIdAndTokenAsync(userId, fcmToken);

        if (token == null)
            return false;

        token.Isactive = false;
        token.Lastupdateddate = DateTime.Now;
        repo.Update(token);
        await UnitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAllUserTokensAsync(int userId)
    {
        await UnitOfWork.UserDeviceTokenRepository.DeactivateTokensByUserIdAsync(userId);
        return true;
    }

    public async Task<IReadOnlyList<UserDeviceTokenBM>> GetActiveTokensByUserIdAsync(int userId)
    {
        var tokens = await UnitOfWork.UserDeviceTokenRepository.GetActiveTokensByUserIdAsync(userId);
        return _mapper.Map<IReadOnlyList<UserDeviceTokenBM>>(tokens);
    }
}


