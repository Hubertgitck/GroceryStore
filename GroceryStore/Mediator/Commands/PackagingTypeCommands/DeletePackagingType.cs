namespace ApplicationWeb.Mediator.Commands.PackagingTypeCommands;

public record DeletePackagingType : IRequest
{
    public readonly int? Id;

	public DeletePackagingType(int? id)
	{
		Id = id;
	}
}