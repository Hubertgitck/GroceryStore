using ApplicationWeb.Mediator.Commands.CartCommands;
using ApplicationWeb.Mediator.Utility;
using Stripe.Checkout;

namespace ApplicationWeb.Mediator.Handlers.CartHandlers;

public class SummaryPostHandler : IRequestHandler<SummaryPost, string>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;
	private OrderHeaderDto _orderHeaderDto;

	public SummaryPostHandler(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
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

			//Stripe settings
			var domain = request.ShoppingCartViewDto.PaymentDomain;

			var options = PrepareStripeOptions(request.ShoppingCartViewDto.PaymentDomain, cartListFromDb);

			var service = new SessionService();
			Session session = service.Create(options);

			_unitOfWork.OrderHeader.UpdateStripePaymentID(_orderHeaderDto.Id,
				session.Id, session.PaymentIntentId);
			_unitOfWork.Save();

			return Task.FromResult(session.Url);
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

	private SessionCreateOptions PrepareStripeOptions(string domain, IEnumerable<ShoppingCart> cartListFromDb)
	{
		var options = new SessionCreateOptions
		{
			PaymentMethodTypes = new List<string>
				{
				"card",
				},
			LineItems = new List<SessionLineItemOptions>(),
			Mode = "payment",
			SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={_orderHeaderDto.Id}",
			CancelUrl = domain + $"customer/cart/index",
		};

		foreach (var item in cartListFromDb)
		{
			{
				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price * 100),
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Name
						},
					},
					Quantity = 1,
				};
				options.LineItems.Add(sessionLineItem);
			}
		}

		return options;
	}
}
