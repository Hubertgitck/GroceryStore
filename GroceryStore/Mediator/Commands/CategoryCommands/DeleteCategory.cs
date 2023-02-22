namespace ApplicationWeb.Mediator.Commands.CategoryCommands;

public record DeleteCategory : IRequest
{
    public readonly int? Id;

	public DeleteCategory(int? id)
	{
		Id = id;
	}
}