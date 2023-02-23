namespace ApplicationWeb.Mediator.Commands.CartCommands;

public record SummaryPost : IRequest<string>
{
	public readonly ClaimsPrincipal ClaimsPrincipal;
	public readonly ShoppingCartViewDto ShoppingCartViewDto;
	public SummaryPost(ClaimsPrincipal claimsPrincipal, ShoppingCartViewDto shoppingCartViewDto)
	{
		ClaimsPrincipal = claimsPrincipal;
		ShoppingCartViewDto = shoppingCartViewDto;
	}
}
