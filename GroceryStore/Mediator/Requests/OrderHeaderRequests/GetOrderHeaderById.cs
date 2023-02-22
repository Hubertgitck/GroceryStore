namespace ApplicationWeb.Mediator.Requests.OrderHeaderRequests;

public record GetOrderHeaderById : IRequest<OrderHeaderDto>
{
    public readonly int Id;

    public GetOrderHeaderById(int id)
    {
        Id = id;
    }
}
