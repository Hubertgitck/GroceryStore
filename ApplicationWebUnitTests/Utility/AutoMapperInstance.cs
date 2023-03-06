using AutoMapper;

namespace ApplicationWebUnitTests.Utility;

public class AutoMapperInstance
{
    public static IMapper GetAutoMapper()
    {
        var myProfile = new MappingProfile();
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
        var mapper = new Mapper(configuration);

        return mapper;
    }
}


