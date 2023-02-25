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

	public SummaryPostHandler(IUnitOfWork unitOfWork, IMapper mapper, IPaymentStrategy paymentStrategy)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
        _paymentStrategy = paymentStrategy;
    }
	public Task<string> Handle(SummaryPost request, CancellationToken cancellationToken)
	{
		var orderHeaderDto = request.ShoppingCartViewDto.OrderHeaderDto;

		var userId = HelperMethods.GetApplicationUserIdFromClaimsPrincipal(request.ClaimsPrincipal);

		var cartListFromDb = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
			includeProperties: "Product");

		if (cartListFromDb.Any())
		{
			var idOfOrderHeaderJustAddedToDb = CreateAndAddOrderHeaderToDb(orderHeaderDto, cartListFromDb, userId);
			CreateAndAddOrderDetailsToDb(cartListFromDb, idOfOrderHeaderJustAddedToDb);

			IPaymentModel stripeModel = new StripeModel()
			{
				OrderId = idOfOrderHeaderJustAddedToDb,
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

    private int CreateAndAddOrderHeaderToDb(OrderHeaderDto orderHeaderDto, IEnumerable<ShoppingCart> cartListFromDb, string userId)
	{
		orderHeaderDto.OrderDate = DateTime.Now;
		orderHeaderDto.ApplicationUserId = userId;

		foreach (var cart in cartListFromDb)
		{
			cart.Price = cart.Count * cart.Product.Price;
			orderHeaderDto.OrderTotal += cart.Price;
		}

		orderHeaderDto.PaymentStatus = Constants.PaymentStatusPending;
		orderHeaderDto.OrderStatus = Constants.StatusPending;

		var orderHeaderToDb = _mapper.Map<OrderHeader>(orderHeaderDto);

		_unitOfWork.OrderHeader.Add(orderHeaderToDb);
		_unitOfWork.Save();

		return orderHeaderToDb.Id;
	}

    private void CreateAndAddOrderDetailsToDb(IEnumerable<ShoppingCart> cartListFromDb, int orderHeaderId)
    {
        foreach (var cart in cartListFromDb)
        {
            OrderDetail orderDetail = new()
            {
                ProductId = cart.ProductId,
                OrderId = orderHeaderId,
                Price = cart.Price,
                Count = cart.Count
            };
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Save();
        }
    }
}
