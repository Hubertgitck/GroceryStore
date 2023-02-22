namespace ApplicationWeb.Mediator.Requests.ShopRequests;

public record GetShopIndexView : IRequest<ShopIndexDto>
{
    public string Category;

    public GetShopIndexView(string category)
    {
        Category = category;
    }
}

