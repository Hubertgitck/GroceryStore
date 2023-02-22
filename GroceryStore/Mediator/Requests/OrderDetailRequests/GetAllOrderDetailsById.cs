namespace ApplicationWeb.Mediator.Requests.OrderDetailRequests;

public record GetAllOrderDetailsById : IRequest<IEnumerable<OrderDetailDto>>
{
    public readonly int Id;

    public GetAllOrderDetailsById(int id)
    {
        Id = id;
    }
}
