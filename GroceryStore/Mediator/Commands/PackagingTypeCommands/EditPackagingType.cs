namespace ApplicationWeb.Mediator.Commands.PackagingTypeCommands;

public record EditPackagingType : IRequest
{
    public readonly PackagingTypeDto PackagingTypeDto;

    public EditPackagingType(PackagingTypeDto packagingTypeDto)
    {
        PackagingTypeDto = packagingTypeDto;
    }
}