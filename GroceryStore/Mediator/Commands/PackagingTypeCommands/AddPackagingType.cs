namespace ApplicationWeb.Mediator.Commands.PackagingTypeCommands;

public record AddPackagingType : IRequest
{
    public PackagingTypeDto PackagingTypeDto;

    public AddPackagingType(PackagingTypeDto packagingTypeDto)
    {
        PackagingTypeDto = packagingTypeDto;
    }
}