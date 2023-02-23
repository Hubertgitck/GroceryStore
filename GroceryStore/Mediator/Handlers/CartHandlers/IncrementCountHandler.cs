using ApplicationWeb.Mediator.Requests.CartRequests;

namespace ApplicationWeb.Mediator.Handlers.CartHandlers;

public class IncrementCountHandler : IRequestHandler<IncrementCount>
{
	private readonly IUnitOfWork _unitOfWork;

	public IncrementCountHandler(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}
	public Task Handle(IncrementCount request, CancellationToken cancellationToken)
	{
		var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == request.Id);
		
		_unitOfWork.ShoppingCart.IncrementCount(cart, 1);
		_unitOfWork.Save();

		return Task.CompletedTask;
	}
}
