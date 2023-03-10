namespace ApplicationWeb.Mediator.Requests.CartRequests;

public record GetSummaryView : IRequest<ShoppingCartViewDto>
{
	public readonly ClaimsPrincipal ClaimsPrincipal;

	public GetSummaryView(ClaimsPrincipal claimsPrincipal)
	{
		ClaimsPrincipal = claimsPrincipal;
	}
}
