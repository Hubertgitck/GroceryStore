namespace ApplicationWeb.Mediator.Requests.CategoryRequests;

public record GetCategoryById : IRequest<CategoryDto>
{
    public readonly int? Id;

    public GetCategoryById(int? id)
    {
        Id = id;
    }
}
