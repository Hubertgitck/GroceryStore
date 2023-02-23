using ApplicationWeb.Mediator.Commands.CartCommands;

namespace ApplicationWeb.Mediator.Handlers.CartHandlers;

public class RemoveCartByIdHandler : IRequestHandler<RemoveCartById>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public RemoveCartByIdHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
	{
		_unitOfWork = unitOfWork;
		_httpContextAccessor = httpContextAccessor;
	}
	public Task Handle(RemoveCartById request, CancellationToken cancellationToken)
	{
		var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == request.Id);
		
		_unitOfWork.ShoppingCart.Remove(cart);
		_unitOfWork.Save();

		var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
		_httpContextAccessor.HttpContext!.Session.SetInt32(Constants.SessionCart, count);

		return Task.CompletedTask;
	}
}
