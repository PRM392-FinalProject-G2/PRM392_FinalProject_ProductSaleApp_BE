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
            CreateMap<ProductSaleApp.API.Models.RequestModel.Auth.RequestOtpRequest, ProductSaleApp.Service.BusinessModel.RequestOtpBM>();
            CreateMap<ProductSaleApp.API.Models.RequestModel.Auth.VerifyOtpRequest, ProductSaleApp.Service.BusinessModel.VerifyOtpBM>();
            CreateMap<ProductSaleApp.API.Models.RequestModel.Auth.ChangePasswordRequest, ProductSaleApp.Service.BusinessModel.ChangePasswordBM>();
            CreateMap<ProductSaleApp.Service.BusinessModel.ResetTokenBM, ProductSaleApp.API.Models.ResponseModel.Auth.ResetTokenResponse>();

            CreateMap<ProductRequest, ProductBM>();
            CreateMap<ProductGetRequest, ProductBM>();
            CreateMap<BrandRequest, BrandBM>();
            CreateMap<BrandGetRequest, BrandBM>();
            CreateMap<CartRequest, CartBM>();
            CreateMap<CartGetRequest, CartBM>();
            CreateMap<CartItemRequest, CartItemBM>();
            CreateMap<CartItemGetRequest, CartItemBM>();
            CreateMap<OrderRequest, OrderBM>();
            CreateMap<OrderGetRequest, OrderBM>();
            CreateMap<PaymentRequest, PaymentBM>();
            CreateMap<PaymentGetRequest, PaymentBM>();
            CreateMap<UserRequest, UserBM>();
            CreateMap<UserGetRequest, UserBM>();
            CreateMap<NotificationRequest, NotificationBM>();
            CreateMap<NotificationGetRequest, NotificationBM>();
            CreateMap<ChatMessageRequest, ChatMessageBM>();
            CreateMap<ChatMessageGetRequest, ChatMessageBM>();
            CreateMap<StoreLocationRequest, StoreLocationBM>();
            CreateMap<VoucherRequest, VoucherBM>();
            CreateMap<VoucherGetRequest, VoucherBM>();
            CreateMap<ProductVoucherRequest, ProductVoucherBM>();
            CreateMap<ProductVoucherGetRequest, ProductVoucherBM>();
            CreateMap<UserVoucherRequest, UserVoucherBM>();
            CreateMap<UserVoucherGetRequest, UserVoucherBM>();
            CreateMap<WishlistRequest, WishlistBM>();
            CreateMap<WishlistGetRequest, WishlistBM>();
            CreateMap<CategoryGetRequest, CategoryBM>();

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
