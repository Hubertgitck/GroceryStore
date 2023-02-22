namespace ApplicationWeb.Mediator.Commands.ShopCommands;
public class DetailsPost : IRequest<string>
{
    public readonly ShoppingCartDto ShoppingCartDto;
    public readonly ClaimsPrincipal ClaimsPrincipal;

    public DetailsPost(ShoppingCartDto shoppingCartDto, ClaimsPrincipal claimsPrincipal)
    {
        ShoppingCartDto = shoppingCartDto;
        ClaimsPrincipal = claimsPrincipal;
    }
}
