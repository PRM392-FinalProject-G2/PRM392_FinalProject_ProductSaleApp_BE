using System;
using System.Threading.Tasks;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

    // Strong-typed repositories
    IProductRepository ProductRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    ICartRepository CartRepository { get; }
    ICartItemRepository CartItemRepository { get; }
    IOrderRepository OrderRepository { get; }
    IPaymentRepository PaymentRepository { get; }
    IUserRepository UserRepository { get; }
    INotificationRepository NotificationRepository { get; }
    IChatMessageRepository ChatMessageRepository { get; }
    IStoreLocationRepository StoreLocationRepository { get; }
    IBrandRepository BrandRepository { get; }
    IVoucherRepository VoucherRepository { get; }
    IProductVoucherRepository ProductVoucherRepository { get; }
    IUserVoucherRepository UserVoucherRepository { get; }
    IWishlistRepository WishlistRepository { get; }

    Task<int> SaveChangesAsync();
}


