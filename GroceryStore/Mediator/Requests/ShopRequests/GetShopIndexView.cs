namespace ApplicationWeb.Mediator.Requests.ShopRequests;

public record GetShopIndexView : IRequest<ShopIndexDto>
{
    public readonly string Category;

    public GetShopIndexView(string category)
    {
        Category = category;
    }
}

