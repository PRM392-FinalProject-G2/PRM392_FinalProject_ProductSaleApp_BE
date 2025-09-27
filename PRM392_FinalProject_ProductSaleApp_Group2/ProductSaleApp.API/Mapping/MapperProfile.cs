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
            CreateMap<ProductRequest, ProductBM>();
            CreateMap<CartRequest, CartBM>();
            CreateMap<CartItemRequest, CartItemBM>();
            CreateMap<OrderRequest, OrderBM>();
            CreateMap<PaymentRequest, PaymentBM>();
            CreateMap<UserRequest, UserBM>();
            CreateMap<NotificationRequest, NotificationBM>();
            CreateMap<ChatMessageRequest, ChatMessageBM>();
            CreateMap<StoreLocationRequest, StoreLocationBM>();

            CreateMap<ProductBM, ProductResponse>();
            CreateMap<CartBM, CartResponse>();
            CreateMap<CartItemBM, CartItemResponse>();
            CreateMap<OrderBM, OrderResponse>();
            CreateMap<PaymentBM, PaymentResponse>();
            CreateMap<UserBM, UserResponse>();
            CreateMap<CategoryBM, CategoryResponse>();
            CreateMap<NotificationBM, NotificationResponse>();
            CreateMap<ChatMessageBM, ChatMessageResponse>();
            CreateMap<StoreLocationBM, StoreLocationResponse>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResponse<>));
        }
    }
}
