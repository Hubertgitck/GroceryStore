using ApplicationWeb.Mediator.Commands.CartCommands;
using ApplicationWeb.Mediator.Utility;
using ApplicationWeb.Payments.Models;
using ApplicationWeb.PaymentServices.Interfaces;

namespace ApplicationWeb.Mediator.Handlers.CartHandlers;

public class SummaryPostHandler : IRequestHandler<SummaryPost, string>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;
    private readonly IPaymentStrategy _paymentStrategy;
    private OrderHeaderDto _orderHeaderDto;

	public SummaryPostHandler(IUnitOfWork unitOfWork, IMapper mapper, IPaymentStrategy paymentStrategy)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
        _paymentStrategy = paymentStrategy;
    }
	public Task<string> Handle(SummaryPost request, CancellationToken cancellationToken)
	{
		_orderHeaderDto = request.ShoppingCartViewDto.OrderHeaderDto;

		var userId = HelperMethods.GetApplicationUserIdFromClaimsPrincipal(request.ClaimsPrincipal);

		var cartListFromDb = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
			includeProperties: "Product");

		if (cartListFromDb.Any())
		{
			PrepareAndAddOrderHeaderToDb(cartListFromDb, userId);

			foreach (var cart in cartListFromDb)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderId = _orderHeaderDto.Id,
					Price = cart.Price,
					Count = cart.Count
				};
				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}
			StripeModel stripeModel = new()
			{
				OrderId = _orderHeaderDto.Id,
				CartList = cartListFromDb
			};

			var redirectUrl = _paymentStrategy.MakePayment(stripeModel);

			return Task.FromResult(redirectUrl);
		}
		else
		{
			return Task.FromResult("");
		}
	}

	private void PrepareAndAddOrderHeaderToDb(IEnumerable<ShoppingCart> cartListFromDb, string userId)
	{
		_orderHeaderDto.OrderDate = DateTime.Now;
		_orderHeaderDto.ApplicationUserId = userId;

		foreach (var cart in cartListFromDb)
		{
			cart.Price = cart.Count * cart.Product.Price;
			_orderHeaderDto.OrderTotal += cart.Price;
		}

		_orderHeaderDto.PaymentStatus = Constants.PaymentStatusPending;
		_orderHeaderDto.OrderStatus = Constants.StatusPending;

		var orderHeaderToDb = _mapper.Map<OrderHeader>(_orderHeaderDto);

		_unitOfWork.OrderHeader.Add(orderHeaderToDb);
		_unitOfWork.Save();

		_orderHeaderDto.Id = orderHeaderToDb.Id;
	}
}
