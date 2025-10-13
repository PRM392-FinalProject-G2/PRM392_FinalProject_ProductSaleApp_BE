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

        // Order mapping: ensure DB snake_case <-> BM camelCase, UserId mapped
        CreateMap<Order, OrderBM>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Orderid))
            .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.Cartid))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Userid))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Paymentmethod))
            .ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => src.Billingaddress))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Orderstatus))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.Orderdate))
            .ReverseMap()
            .ForMember(dest => dest.Orderid, opt => opt.Ignore())
            .ForMember(dest => dest.Cartid, opt => opt.MapFrom(src => src.CartId))
            .ForMember(dest => dest.Userid, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Paymentmethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.Billingaddress, opt => opt.MapFrom(src => src.BillingAddress))
            .ForMember(dest => dest.Orderstatus, opt => opt.MapFrom(src => src.OrderStatus))
            .ForMember(dest => dest.Orderdate, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Cart, opt => opt.Ignore())
            .ForMember(dest => dest.Payments, opt => opt.Ignore())
            .ForMember(dest => dest.Uservouchers, opt => opt.Ignore());

        // Payment mapping: ensure OrderId maps to Orderid on save
        CreateMap<Payment, PaymentBM>()
            .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Paymentid))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Orderid))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Paymentstatus))
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.Paymentdate))
            .ReverseMap()
            .ForMember(dest => dest.Paymentid, opt => opt.Ignore())
            .ForMember(dest => dest.Orderid, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Order, opt => opt.Ignore());
        
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
        CreateMap<Uservoucher, UserVoucherBM>()
     .ForMember(dest => dest.UserVoucherId, opt => opt.MapFrom(src => src.Uservoucherid))
     .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Userid))
     .ForMember(dest => dest.VoucherId, opt => opt.MapFrom(src => src.Voucherid))
     .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Orderid))
     .ForMember(dest => dest.IsUsed, opt => opt.MapFrom(src => src.Isused))
     .ForMember(dest => dest.UsedAt, opt => opt.MapFrom(src => src.Usedat))
     .ReverseMap()
     .ForMember(dest => dest.Uservoucherid, opt => opt.Ignore()) 
     .ForMember(dest => dest.Userid, opt => opt.MapFrom(src => src.UserId))
     .ForMember(dest => dest.Voucherid, opt => opt.MapFrom(src => src.VoucherId))
     .ForMember(dest => dest.Orderid, opt => opt.MapFrom(src => src.OrderId))
     .ForMember(dest => dest.Isused, opt => opt.MapFrom(src => src.IsUsed))
     .ForMember(dest => dest.Usedat, opt => opt.MapFrom(src => src.UsedAt))
     .ForMember(dest => dest.User, opt => opt.Ignore())
     .ForMember(dest => dest.Voucher, opt => opt.Ignore())
     .ForMember(dest => dest.Order, opt => opt.Ignore());

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


