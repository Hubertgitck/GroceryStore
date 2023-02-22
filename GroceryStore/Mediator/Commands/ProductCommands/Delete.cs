namespace ApplicationWeb.Mediator.Commands.ProductCommands;

public record Delete : IRequest<Dictionary<string, string>>
{
    public readonly int? Id;

	public Delete(int? id)
	{
		Id = id;
	}
}