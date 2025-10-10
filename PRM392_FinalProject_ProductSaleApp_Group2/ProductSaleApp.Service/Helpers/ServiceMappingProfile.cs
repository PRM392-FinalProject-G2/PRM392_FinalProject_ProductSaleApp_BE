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
        
        // Product mapping - ignore navigation collections when mapping back
        CreateMap<Product, ProductBM>()
            .ReverseMap()
            .ForMember(dest => dest.Cartitems, opt => opt.Ignore())
            .ForMember(dest => dest.Productvouchers, opt => opt.Ignore())
            .ForMember(dest => dest.Wishlists, opt => opt.Ignore())
            .ForMember(dest => dest.Productimages, opt => opt.Ignore())
            .ForMember(dest => dest.Productreviews, opt => opt.Ignore());
        
        CreateMap<Brand, BrandBM>().ReverseMap();
        CreateMap<Cart, CartBM>().ReverseMap();
        CreateMap<Cartitem, CartItemBM>().ReverseMap();
        CreateMap<Order, OrderBM>().ReverseMap();
        CreateMap<Payment, PaymentBM>().ReverseMap();
        
        // User mapping with explicit property mapping for case-sensitive fields
        CreateMap<User, UserBM>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Userid))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Passwordhash))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phonenumber))
            .ForMember(dest => dest.Avatarurl, opt => opt.MapFrom(src => src.Avatarurl))
            .ReverseMap()
            .ForMember(dest => dest.Userid, opt => opt.Ignore()) // IGNORE primary key - cannot be modified
            .ForMember(dest => dest.Passwordhash, opt => opt.MapFrom(src => src.PasswordHash))
            .ForMember(dest => dest.Phonenumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Avatarurl, opt => opt.MapFrom(src => src.Avatarurl));
        
        CreateMap<Notification, NotificationBM>().ReverseMap();
        CreateMap<Chatmessage, ChatMessageBM>().ReverseMap();
        CreateMap<Storelocation, StoreLocationBM>().ReverseMap();
        CreateMap<Voucher, VoucherBM>().ReverseMap();
        CreateMap<Productvoucher, ProductVoucherBM>().ReverseMap();
        CreateMap<Uservoucher, UserVoucherBM>().ReverseMap();
        CreateMap<Wishlist, WishlistBM>().ReverseMap();
        
        // ProductImage mapping - ignore navigation property to avoid circular reference
        CreateMap<Productimage, ProductImageBM>()
            .ReverseMap()
            .ForMember(dest => dest.Product, opt => opt.Ignore());
        
        // ProductReview mapping - include User but ignore Product navigation property
        CreateMap<Productreview, ProductReviewBM>()
            .ReverseMap()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}


