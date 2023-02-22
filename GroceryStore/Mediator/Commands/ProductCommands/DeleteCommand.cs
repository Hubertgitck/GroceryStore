namespace ApplicationWeb.Mediator.Commands.ProductCommands;

public record DeleteCommand : IRequest<Dictionary<string, string>>
{
    public readonly int? Id;

	public DeleteCommand(int? id)
	{
		Id = id;
	}
}