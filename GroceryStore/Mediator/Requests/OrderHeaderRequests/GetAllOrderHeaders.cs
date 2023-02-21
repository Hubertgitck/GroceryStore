namespace ApplicationWeb.Mediator.Requests.OrderHeaderRequests;

public record GetAllOrderHeaders : IRequest<IEnumerable<OrderHeaderDto>>
{
    public readonly ClaimsPrincipal Claim;
    public readonly string Status;

    public GetAllOrderHeaders(ClaimsPrincipal claim, string status)
    {
        Claim = claim;
        Status = status;
    }
}
