namespace ApplicationWeb.Mediator.Commands.CartCommands;

public record RemoveCartById : IRequest
{
	public readonly int Id;

	public RemoveCartById(int id)
	{
		Id = id;
	}
}
