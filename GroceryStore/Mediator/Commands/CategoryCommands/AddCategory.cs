namespace ApplicationWeb.Mediator.Commands.CategoryCommands;

public record AddCategory : IRequest
{
    public CategoryDto Category;

    public AddCategory(CategoryDto category)
    {
        Category = category;
    }
}