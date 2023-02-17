namespace ApplicationWeb.Mediator.Commands.CategoryCommands;

public record EditCategory : IRequest
{
    public CategoryDto Category;

    public EditCategory(CategoryDto category)
    {
        Category = category;
    }
}