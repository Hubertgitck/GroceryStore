namespace ApplicationWeb.Mediator.Requests.OrderHeaderRequests;

public record GetAllOrderHeaders : IRequest<IEnumerable<OrderHeaderDto>>
{
    public readonly ClaimsPrincipal ClaimsPrincipal;
    public readonly string Status;

    public GetAllOrderHeaders(ClaimsPrincipal claimsPrincipal, string status)
    {
        ClaimsPrincipal = claimsPrincipal;
        Status = status;
    }
}
