using AutoMapper;

namespace ApplicationWeb.Mediator.DTO;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, Category>();
    }
}
