namespace ApplicationWeb.Mediator.Commands.CategoryCommands;

public record AddCategory : IRequest
{
    public CategoryDto CategoryDto;

    public AddCategory(CategoryDto category)
    {
        CategoryDto = category;
    }
}
