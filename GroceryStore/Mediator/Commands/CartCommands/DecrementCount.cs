namespace ApplicationWeb.Mediator.Commands.CartCommands;

public record DecrementCount : IRequest
{
	public readonly int Id;

	public DecrementCount(int id)
	{
		Id = id;
	}
}
