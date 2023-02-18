
using Application.Models;
using ApplicationWeb.Mediator.Commands.OrderHeaderCommands;
using ApplicationWeb.Mediator.Requests.OrderHeaderRequests;
using AutoMapper;
using Stripe.Checkout;

namespace ApplicationWeb.Mediator.Handlers.OrderHeaderHandlers;

public class PaymentConfirmationHandler : IRequestHandler<PaymentConfirmation>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StripeServiceProvider _stripeServices;

    public PaymentConfirmationHandler(IUnitOfWork unitOfWork, StripeServiceProvider stripeServices)
    {
        _unitOfWork = unitOfWork;
        _stripeServices = stripeServices;
    }

    public Task Handle(PaymentConfirmation request, CancellationToken cancellationToken)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == request.Id);

        Session session = _stripeServices.GetStripeSession(orderHeader.SessionId);

        if (session.PaymentStatus.ToLower() == "paid")
        {
            _unitOfWork.OrderHeader.UpdateStripePaymentID(request.Id, orderHeader.SessionId, session.PaymentIntentId);
            _unitOfWork.OrderHeader.UpdateStatus(request.Id, orderHeader.OrderStatus, Constants.PaymentStatusApproved);
            _unitOfWork.Save();
        }

        return Task.CompletedTask;
    }
}
