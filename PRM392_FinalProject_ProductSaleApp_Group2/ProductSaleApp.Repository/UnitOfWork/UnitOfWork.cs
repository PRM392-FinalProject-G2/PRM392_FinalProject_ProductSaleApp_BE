using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ProductSaleApp.Repository.DBContext;
using ProductSaleApp.Repository.Repositories.Implementations;
using ProductSaleApp.Repository.Repositories.Interfaces;

namespace ProductSaleApp.Repository.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly SalesAppDBContext _dbContext;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    
    // Strong-typed repositories backing fields
    private IProductRepository _productRepository;
    private IProductImageRepository _productImageRepository;
    private IProductReviewRepository _productReviewRepository;
    private ICategoryRepository _categoryRepository;
    private ICartRepository _cartRepository;
    private ICartItemRepository _cartItemRepository;
    private IOrderRepository _orderRepository;
    private IPaymentRepository _paymentRepository;
    private IUserRepository _userRepository;
    private INotificationRepository _notificationRepository;
    private IChatMessageRepository _chatMessageRepository;
    private IStoreLocationRepository _storeLocationRepository;
    private IBrandRepository _brandRepository;
    private IVoucherRepository _voucherRepository;
    private IProductVoucherRepository _productVoucherRepository;
    private IUserVoucherRepository _userVoucherRepository;
    private IWishlistRepository _wishlistRepository;
    private IUserDeviceTokenRepository _userDeviceTokenRepository;

    public UnitOfWork(SalesAppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public SalesAppDBContext DbContext => _dbContext;

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);
        if (_repositories.ContainsKey(type))
        {
            return (IGenericRepository<TEntity>)_repositories[type];
        }

        var repository = new GenericRepository<TEntity>(_dbContext);
        _repositories[type] = repository;
        return repository;
    }

    public IProductRepository ProductRepository => _productRepository ??= new ProductRepository(_dbContext);
    public IProductImageRepository ProductImageRepository => _productImageRepository ??= new ProductImageRepository(_dbContext);
    public IProductReviewRepository ProductReviewRepository => _productReviewRepository ??= new ProductReviewRepository(_dbContext);
    public ICategoryRepository CategoryRepository => _categoryRepository ??= new CategoryRepository(_dbContext);
    public ICartRepository CartRepository => _cartRepository ??= new CartRepository(_dbContext);
    public ICartItemRepository CartItemRepository => _cartItemRepository ??= new CartItemRepository(_dbContext);
    public IOrderRepository OrderRepository => _orderRepository ??= new OrderRepository(_dbContext);
    public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_dbContext);
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(_dbContext);
    public INotificationRepository NotificationRepository => _notificationRepository ??= new NotificationRepository(_dbContext);
    public IChatMessageRepository ChatMessageRepository => _chatMessageRepository ??= new ChatMessageRepository(_dbContext);
    public IStoreLocationRepository StoreLocationRepository => _storeLocationRepository ??= new StoreLocationRepository(_dbContext);
    public IBrandRepository BrandRepository => _brandRepository ??= new BrandRepository(_dbContext);
    public IVoucherRepository VoucherRepository => _voucherRepository ??= new VoucherRepository(_dbContext);
    public IProductVoucherRepository ProductVoucherRepository => _productVoucherRepository ??= new ProductVoucherRepository(_dbContext);
    public IUserVoucherRepository UserVoucherRepository => _userVoucherRepository ??= new UserVoucherRepository(_dbContext);
    public IWishlistRepository WishlistRepository => _wishlistRepository ??= new WishlistRepository(_dbContext);
    public IUserDeviceTokenRepository UserDeviceTokenRepository => _userDeviceTokenRepository ??= new UserDeviceTokenRepository(_dbContext);

    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}


