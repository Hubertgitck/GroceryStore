namespace ApplicationWeb.Mediator.Requests.OrderHeaderRequests;

public record GetOrderHeaderById : IRequest<OrderHeaderDto>
{
    public int Id;

    public GetOrderHeaderById(int id)
    {
        Id = id;
    }
}
