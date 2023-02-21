namespace ApplicationWeb.Mediator.Commands.PackagingTypeCommands;

public record DeletePackagingType : IRequest
{
    public int? Id;

	public DeletePackagingType(int? id)
	{
		Id = id;
	}
}