﻿using ApplicationWeb.Mediator.Requests.CartRequests;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe.Checkout;

namespace ApplicationWeb.Mediator.Handlers.CartHandlers;

public class OrderConfirmationHandler : IRequestHandler<OrderConfirmation>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;
	private readonly StripeServiceProvider _stripeServiceProvider;
	private readonly IEmailSender _emailSender;

	public OrderConfirmationHandler(IUnitOfWork unitOfWork, IMapper mapper, 
		StripeServiceProvider stripeServiceProvider, IEmailSender emailSender)
	{
		_unitOfWork = unitOfWork;
		_mapper = mapper;
		_stripeServiceProvider = stripeServiceProvider;
		_emailSender = emailSender;
	}
	public Task Handle(OrderConfirmation request, CancellationToken cancellationToken)
	{
		OrderHeader orderHeader = _unitOfWork.OrderHeader
			.GetFirstOrDefault(u => u.Id == request.Id, includeProperties: "ApplicationUser");

		var session = _stripeServiceProvider.GetStripeSession(orderHeader.SessionId);

		if (session.PaymentStatus.ToLower() == "paid")
		{
			_unitOfWork.OrderHeader.UpdatePaymentID(request.Id, orderHeader.SessionId, session.PaymentIntentId);
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
