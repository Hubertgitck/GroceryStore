namespace ApplicationWeb.Mediator.Requests;

public class AddCategory : IRequest<string>
{
    public Category Category;

    public AddCategory(Category category)
    {
        Category = category;
    }
}