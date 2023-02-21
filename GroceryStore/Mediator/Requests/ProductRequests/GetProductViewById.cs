namespace ApplicationWeb.Mediator.Requests.ProductRequests;

public class GetProductViewById : IRequest<ProductViewDto>
{
    public int? Id;

    public GetProductViewById(int? id)
    {
        Id = id;
    }
}
