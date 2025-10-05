using AutoMapper;
using ProductSaleApp.Repository.Models;
using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.Service.Helpers;

public class ServiceMappingProfile : Profile
{
    public ServiceMappingProfile()
    {
            // Auth BMs map to Repository models where needed are handled in service, not here
        CreateMap<Category, CategoryBM>().ReverseMap();
        CreateMap<Product, ProductBM>().ReverseMap();
        CreateMap<Brand, BrandBM>().ReverseMap();
        CreateMap<Cart, CartBM>().ReverseMap();
        CreateMap<Cartitem, CartItemBM>().ReverseMap();
        CreateMap<Order, OrderBM>().ReverseMap();
        CreateMap<Payment, PaymentBM>().ReverseMap();
        CreateMap<User, UserBM>().ReverseMap();
        CreateMap<Notification, NotificationBM>().ReverseMap();
        CreateMap<Chatmessage, ChatMessageBM>().ReverseMap();
        CreateMap<Storelocation, StoreLocationBM>().ReverseMap();
        CreateMap<Voucher, VoucherBM>().ReverseMap();
        CreateMap<Productvoucher, ProductVoucherBM>().ReverseMap();
        CreateMap<Uservoucher, UserVoucherBM>().ReverseMap();
        CreateMap<Wishlist, WishlistBM>().ReverseMap();
    }
}


