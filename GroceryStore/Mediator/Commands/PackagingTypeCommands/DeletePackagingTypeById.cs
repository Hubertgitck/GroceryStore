namespace ApplicationWeb.Mediator.Commands.PackagingTypeCommands;

public record DeletePackagingTypeById : IRequest
{
    public readonly int? Id;

	public DeletePackagingTypeById(int? id)
	{
		Id = id;
	}
}