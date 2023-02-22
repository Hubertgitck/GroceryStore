namespace ApplicationWeb.Mediator.Commands.PackagingTypeCommands;

public record AddPackagingType : IRequest
{
    public readonly PackagingTypeDto PackagingTypeDto;

    public AddPackagingType(PackagingTypeDto packagingTypeDto)
    {
        PackagingTypeDto = packagingTypeDto;
    }
}