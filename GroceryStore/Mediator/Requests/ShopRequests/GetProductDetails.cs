namespace ApplicationWeb.Mediator.Requests.ShopRequests;

public record GetProductDetails : IRequest<ShoppingCartDto>
{
    public readonly int Id;
    public readonly ClaimsPrincipal ClaimsPrincipal;

    public GetProductDetails(ClaimsPrincipal claimsPrincipal, int id)
    {
        ClaimsPrincipal = claimsPrincipal;
        Id = id;
    }
}

