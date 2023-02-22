using ApplicationWeb.Mediator.Requests.ShopRequests;
using ApplicationWeb.Mediator.Utility;

namespace ApplicationWeb.Mediator.Handlers.ShopHandlers;

public class GetProductDetailsHandler : IRequestHandler<GetProductDetails, ShoppingCartDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductDetailsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<ShoppingCartDto> Handle(GetProductDetails request, CancellationToken cancellationToken)
    {
        ShoppingCartDto shoppingCartDto = new();
        var userId = HelperMethods.GetApplicationUserIdFromClaimsPrincipal(request.ClaimsPrincipal);

        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart
            .GetFirstOrDefault(u => u.ApplicationUserId == userId && u.ProductId == request.Id);

        Product product = _unitOfWork.Product
            .GetFirstOrDefault(u => u.Id == request.Id, includeProperties: "Category,PackagingType");

        if (cartFromDb != null)
        {
            cartFromDb.Product = product;
            shoppingCartDto = _mapper.Map<ShoppingCartDto>(cartFromDb);
        }
        else
        {
            shoppingCartDto = new()
            {
                Count = 1,
                ProductId = request.Id,
                ProductDto = _mapper.Map<ProductDto>(product)
            };
        }
        return await Task.FromResult(shoppingCartDto);
    }
}
