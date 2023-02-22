namespace ApplicationWeb.Mediator.Requests.CartRequests;

public record GetCartIndexView : IRequest<ShoppingCartViewDto>
{
	public readonly ClaimsPrincipal ClaimsPrincipal;

	public GetCartIndexView(ClaimsPrincipal claimsPrincipal)
	{
		ClaimsPrincipal = claimsPrincipal;
	}
}
