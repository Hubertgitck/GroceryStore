namespace ApplicationWeb.Mediator.Requests.CategoryRequests;

public record GetCategoryById : IRequest<CategoryDto>
{
    public int? Id;

    public GetCategoryById(int? id)
    {
        Id = id;
    }
}
