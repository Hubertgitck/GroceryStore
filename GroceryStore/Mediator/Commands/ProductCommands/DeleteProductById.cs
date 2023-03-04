namespace ApplicationWeb.Mediator.Commands.ProductCommands;

public record DeleteProductById : IRequest<Dictionary<string, string>>
{
    public readonly int? Id;

	public DeleteProductById(int? id)
	{
		Id = id;
	}
}