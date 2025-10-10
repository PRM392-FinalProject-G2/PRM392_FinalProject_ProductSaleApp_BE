using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Service.Services.Implementations;
using ProductSaleApp.Service.Services.Interfaces;
using ProductSaleApp.Repository.Repositories.Interfaces;
using ProductSaleApp.Repository.DBContext;
using CloudinaryDotNet;

var builder = WebApplication.CreateBuilder(args);
// Cấu hình để chạy trên Docker/Render
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080); // Render yêu cầu chạy ở port 8080
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add Cloudinary
var cloudinaryAccount = new Account(
    builder.Configuration["CloudinarySettings:CloudName"],
    builder.Configuration["CloudinarySettings:ApiKey"],
    builder.Configuration["CloudinarySettings:ApiSecret"]
);
var cloudinary = new Cloudinary(cloudinaryAccount);
builder.Services.AddSingleton(cloudinary);

// Add DbContext
builder.Services.AddDbContext<SalesAppDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:3000",
                    "http://127.0.0.1:5500",
                    "http://localhost:5500"
                 )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
});
// Unit of Work / Repositories
builder.Services.AddScoped<ProductSaleApp.Repository.UnitOfWork.IUnitOfWork, ProductSaleApp.Repository.UnitOfWork.UnitOfWork>();

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile(new ProductSaleApp.Service.Helpers.ServiceMappingProfile());
    cfg.AddProfile(new ProductSaleApp.API.Mapping.MapperProfile());
});

// Services
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<IStoreLocationService, StoreLocationService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddScoped<IProductVoucherService, ProductVoucherService>();
builder.Services.AddScoped<IUserVoucherService, UserVoucherService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();

// Repositories
// Only UoW; repositories accessed via UoW

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
