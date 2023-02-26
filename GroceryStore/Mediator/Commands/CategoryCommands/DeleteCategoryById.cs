namespace ApplicationWeb.Mediator.Commands.CategoryCommands;

public record DeleteCategoryById : IRequest
{
    public readonly int? Id;

	public DeleteCategoryById(int? id)
	{
		Id = id;
	}
}