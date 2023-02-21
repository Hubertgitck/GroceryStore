namespace ApplicationWeb.Mediator.Commands.PackagingTypeCommands;

public record EditPackagingType : IRequest
{
    public PackagingTypeDto PackagingTypeDto;

    public EditPackagingType(PackagingTypeDto packagingTypeDto)
    {
        PackagingTypeDto = packagingTypeDto;
    }
}