using AutoMapper;
using ProductSaleApp.API.Models.RequestModel;
using ProductSaleApp.API.Models.ResponseModel;
using ProductSaleApp.Service.BusinessModel;

namespace ProductSaleApp.API.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Auth
            CreateMap<ProductSaleApp.API.Models.RequestModel.Auth.LoginRequest, ProductSaleApp.Service.BusinessModel.LoginBM>();
            CreateMap<ProductSaleApp.API.Models.RequestModel.Auth.RegisterRequest, ProductSaleApp.Service.BusinessModel.RegisterBM>();
            CreateMap<ProductSaleApp.Service.BusinessModel.AuthBM, ProductSaleApp.API.Models.ResponseModel.Auth.AuthResponse>();

            CreateMap<ProductRequest, ProductBM>();
            CreateMap<BrandRequest, BrandBM>();
            CreateMap<CartRequest, CartBM>();
            CreateMap<CartItemRequest, CartItemBM>();
            CreateMap<OrderRequest, OrderBM>();
            CreateMap<PaymentRequest, PaymentBM>();
            CreateMap<UserRequest, UserBM>();
            CreateMap<NotificationRequest, NotificationBM>();
            CreateMap<ChatMessageRequest, ChatMessageBM>();
            CreateMap<StoreLocationRequest, StoreLocationBM>();
            CreateMap<VoucherRequest, VoucherBM>();
            CreateMap<ProductVoucherRequest, ProductVoucherBM>();
            CreateMap<UserVoucherRequest, UserVoucherBM>();
            CreateMap<WishlistRequest, WishlistBM>();

            CreateMap<ProductBM, ProductResponse>();
            CreateMap<BrandBM, BrandResponse>();
            CreateMap<CartBM, CartResponse>();
            CreateMap<CartItemBM, CartItemResponse>();
            CreateMap<OrderBM, OrderResponse>();
            CreateMap<PaymentBM, PaymentResponse>();
            CreateMap<UserBM, UserResponse>();
            CreateMap<CategoryBM, CategoryResponse>();
            CreateMap<NotificationBM, NotificationResponse>();
            CreateMap<ChatMessageBM, ChatMessageResponse>();
            CreateMap<StoreLocationBM, StoreLocationResponse>();
            CreateMap<VoucherBM, VoucherResponse>();
            CreateMap<ProductVoucherBM, ProductVoucherResponse>();
            CreateMap<UserVoucherBM, UserVoucherResponse>();
            CreateMap<WishlistBM, WishlistResponse>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResponse<>));
        }
    }
}
