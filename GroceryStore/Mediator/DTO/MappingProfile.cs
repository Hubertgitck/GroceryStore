using AutoMapper;

namespace ApplicationWeb.Mediator.DTO;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //  <source,destination>

        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();

        CreateMap<OrderDetail, OrderDetailDto>()
            .ForMember(dest => dest.ProductDto, opt => opt.MapFrom(src => src.Product))
            .ForMember(dest => dest.OrderHeaderDto, opt => opt.MapFrom(src => src.OrderHeader));

        CreateMap<ApplicationUser, ApplicationUserDto>();

        CreateMap<Product, ProductDto>();
        CreateMap<ProductDto, Product>();

        CreateMap<OrderHeaderDto, OrderHeader>();
        CreateMap<OrderHeader, OrderHeaderDto>()
            .ForMember(dest => dest.ApplicationUserDto, opt => opt.MapFrom(src => src.ApplicationUser));

        CreateMap<PackagingType, PackagingTypeDto>();
        CreateMap<PackagingTypeDto, PackagingType>();

        CreateMap<ShoppingCart, ShoppingCartDto>()
            .ForMember(dest => dest.ProductDto, opt => opt.MapFrom(src => src.Product));
        CreateMap<ShoppingCartDto, ShoppingCart>();
    }
}
