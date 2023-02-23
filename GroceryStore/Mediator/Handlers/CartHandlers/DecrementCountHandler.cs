using ApplicationWeb.Mediator.Commands.CartCommands;

namespace ApplicationWeb.Mediator.Handlers.CartHandlers;

public class DecrementCountHandler : IRequestHandler<DecrementCount>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public DecrementCountHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
	{
		_unitOfWork = unitOfWork;
		_httpContextAccessor = httpContextAccessor;
	}
	public Task Handle(DecrementCount request, CancellationToken cancellationToken)
	{
		var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == request.Id);
		if (cart.Count <= 1)
		{
			_unitOfWork.ShoppingCart.Remove(cart);
			var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
			_httpContextAccessor.HttpContext!.Session.SetInt32(Constants.SessionCart, count);
		}
		else
		{
			_unitOfWork.ShoppingCart.DecrementCount(cart, 1);
		}

		_unitOfWork.Save();

		return Task.CompletedTask;
	}
}
