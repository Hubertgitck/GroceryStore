using ApplicationWeb.Mediator.Requests.CartRequests;
using ApplicationWeb.Mediator.Utility;

namespace ApplicationWeb.Mediator.Handlers.CartHandlers;

public class GetSummaryViewHandler : IRequestHandler<GetSummaryView, ShoppingCartViewDto>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;

	public GetSummaryViewHandler(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
	}
	public Task<ShoppingCartViewDto> Handle(GetSummaryView request, CancellationToken cancellationToken)
	{
		var userId = HelperMethods.GetApplicationUserIdFromClaimsPrincipal(request.ClaimsPrincipal);
		var shoppingCartColllection = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
				includeProperties: "Product", thenIncludeProperty: "PackagingType");

		ShoppingCartViewDto shoppingCartViewDto = new ()
		{
			CartList = _mapper.Map<IEnumerable<ShoppingCartDto>>(shoppingCartColllection),
			OrderHeaderDto = new()
		};

		var applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);

		shoppingCartViewDto.OrderHeaderDto.ApplicationUserDto = _mapper.Map<ApplicationUserDto>(applicationUser);
		shoppingCartViewDto.OrderHeaderDto = _mapper.Map<OrderHeaderDto>(shoppingCartViewDto.OrderHeaderDto.ApplicationUserDto);

		foreach (var cart in shoppingCartViewDto.CartList)
		{
			cart.Price = cart.Count * cart.ProductDto.Price;
			shoppingCartViewDto.OrderHeaderDto.OrderTotal += cart.Price;
		}

		return Task.FromResult(shoppingCartViewDto);
	}
}
