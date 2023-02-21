namespace ApplicationWeb.Mediator.Commands.CategoryCommands;

public record EditCategory : IRequest
{
    public CategoryDto CategoryDto;

    public EditCategory(CategoryDto category)
    {
        CategoryDto = category;
    }
}