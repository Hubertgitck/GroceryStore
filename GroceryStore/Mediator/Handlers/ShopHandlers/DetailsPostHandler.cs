using ApplicationWeb.Mediator.Commands.ShopCommands;
using ApplicationWeb.Mediator.Utility;

namespace ApplicationWeb.Mediator.Handlers.ShopHandlers;

public class DetailsPostHandler : IRequestHandler<DetailsPost, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DetailsPostHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<string> Handle(DetailsPost request, CancellationToken cancellationToken)
    {
        string message;
        var userId = HelperMethods.GetApplicationUserIdFromClaimsPrincipal(request.ClaimsPrincipal);

        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart
            .GetFirstOrDefault(u => u.ApplicationUserId == userId && u.ProductId == request.ShoppingCartDto.ProductId);

        if (cartFromDb == null)
        {
            var shoppingCartToDb = _mapper.Map<ShoppingCart>(request.ShoppingCartDto);
            shoppingCartToDb.ApplicationUserId = userId;
            _unitOfWork.ShoppingCart.Add(shoppingCartToDb);
            _unitOfWork.Save();
            message = "Product added to cart succesfully";
            _httpContextAccessor.HttpContext!.Session.SetInt32(Constants.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).ToList().Count());
        }
        else
        {
            cartFromDb.Count = request.ShoppingCartDto.Count;
            message = "Shopping Cart updated succesfully";
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
        }

        return await Task.FromResult(message);
    }
}
