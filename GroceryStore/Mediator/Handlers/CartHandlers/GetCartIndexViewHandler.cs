using ApplicationWeb.Mediator.Requests.CartRequests;
using ApplicationWeb.Mediator.Utility;

namespace ApplicationWeb.Mediator.Handlers.CartHandlers;

public class GetCartIndexViewHandler : IRequestHandler<GetCartIndexView, ShoppingCartViewDto>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;

	public GetCartIndexViewHandler(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}
	public Task<ShoppingCartViewDto> Handle(GetCartIndexView request, CancellationToken cancellationToken)
	{
		var userId = HelperMethods.GetApplicationUserIdFromClaimsPrincipal(request.ClaimsPrincipal);
		var shoppingCartColllection = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
				includeProperties: "Product");

		ShoppingCartViewDto shoppingCartViewDto = new()
		{
			CartList = _mapper.Map<IEnumerable<ShoppingCartDto>>(shoppingCartColllection),
			OrderHeaderDto = new()
		};

		foreach (var cart in shoppingCartViewDto.CartList)
		{
			shoppingCartViewDto.OrderHeaderDto.OrderTotal += (cart.Count * cart.ProductDto.Price);
		}

		return Task.FromResult(shoppingCartViewDto);
	}
}
