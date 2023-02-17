namespace ApplicationWeb.Mediator.Commands.CategoryCommands;

public record DeleteCategory : IRequest
{
    public int? Id;

	public DeleteCategory(int? id)
	{
		Id = id;
	}
}