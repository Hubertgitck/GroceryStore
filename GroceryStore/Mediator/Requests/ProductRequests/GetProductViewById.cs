namespace ApplicationWeb.Mediator.Requests.ProductRequests;

public class GetProductViewById : IRequest<ProductViewDto>
{
    public readonly int? Id;

    public GetProductViewById(int? id)
    {
        Id = id;
    }
}
