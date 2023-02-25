using ApplicationWeb.Mediator.Requests.CartRequests;
using ApplicationWeb.Payments.Models;
using ApplicationWeb.PaymentServices.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe.Checkout;

namespace ApplicationWeb.Mediator.Handlers.CartHandlers;

public class OrderConfirmationHandler : IRequestHandler<OrderConfirmation>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IEmailSender _emailSender;
    private readonly IPaymentStrategy _paymentStrategy;

    public OrderConfirmationHandler(IUnitOfWork unitOfWork, IEmailSender emailSender, IPaymentStrategy paymentStrategy)
	{
		_unitOfWork = unitOfWork;
		_emailSender = emailSender;
        _paymentStrategy = paymentStrategy;
    }
	public Task Handle(OrderConfirmation request, CancellationToken cancellationToken)
	{
		OrderHeader orderHeader = _unitOfWork.OrderHeader
			.GetFirstOrDefault(u => u.Id == request.Id, includeProperties: "ApplicationUser");

		var paymentStatus = _paymentStrategy.GetPaymentStatus(new StripeModel
		{
			SessionId = orderHeader.SessionId
		});

		if (paymentStatus == "paid")
		{
			_unitOfWork.OrderHeader.UpdatePaymentID(request.Id, orderHeader.SessionId, orderHeader.PaymentIntendId);
			_unitOfWork.OrderHeader.UpdateStatus(request.Id, Constants.StatusApproved, Constants.PaymentStatusApproved);
		}

		_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Grocery Store", "<p>New Order Created</p>");

		List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
			.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
		
		_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
		_unitOfWork.Save();

		return Task.CompletedTask;
	}
}
