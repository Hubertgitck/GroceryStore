namespace ApplicationWeb.Mediator.Requests.ProductRequests;

public record GetAllProducts : IRequest<IEnumerable<ProductDto>>
{

}
