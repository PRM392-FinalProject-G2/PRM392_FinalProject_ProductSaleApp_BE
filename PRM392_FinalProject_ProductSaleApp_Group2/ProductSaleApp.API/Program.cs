using Microsoft.EntityFrameworkCore;
using ProductSaleApp.Service.Services.Implementations;
using ProductSaleApp.Service.Services.Interfaces;
using ProductSaleApp.Repository.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<ProductSaleApp.Repository.DBContext.SalesAppDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
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

// Repositories
// Only UoW; repositories accessed via UoW

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
