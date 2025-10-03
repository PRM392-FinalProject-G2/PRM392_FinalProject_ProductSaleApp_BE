using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Helpers;

public class ServiceMappingProfile : Profile
{
    public ServiceMappingProfile()
    {
        CreateMap<Category, CategoryBM>().ReverseMap();
        CreateMap<Product, ProductBM>().ReverseMap();
        CreateMap<Brand, BrandBM>().ReverseMap();
        CreateMap<Cart, CartBM>().ReverseMap();
        CreateMap<CartItem, CartItemBM>().ReverseMap();
        CreateMap<Order, OrderBM>().ReverseMap();
        CreateMap<Payment, PaymentBM>().ReverseMap();
        CreateMap<User, UserBM>().ReverseMap();
        CreateMap<Notification, NotificationBM>().ReverseMap();
        CreateMap<ChatMessage, ChatMessageBM>().ReverseMap();
        CreateMap<StoreLocation, StoreLocationBM>().ReverseMap();
        CreateMap<Voucher, VoucherBM>().ReverseMap();
        CreateMap<ProductVoucher, ProductVoucherBM>().ReverseMap();
        CreateMap<UserVoucher, UserVoucherBM>().ReverseMap();
        CreateMap<Wishlist, WishlistBM>().ReverseMap();
    }
}


