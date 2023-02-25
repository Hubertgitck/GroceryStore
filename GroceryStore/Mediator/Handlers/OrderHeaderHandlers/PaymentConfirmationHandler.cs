using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Commands.OrderHeaderCommands;
using ApplicationWeb.Payments.Models;
using ApplicationWeb.PaymentServices.Interfaces;
using Stripe.Checkout;

namespace ApplicationWeb.Mediator.Handlers.OrderHeaderHandlers;

public class PaymentConfirmationHandler : IRequestHandler<PaymentConfirmation>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentStrategy _paymentStrategy;

    public PaymentConfirmationHandler(IUnitOfWork unitOfWork, IPaymentStrategy paymentStrategy)
    {
        _unitOfWork = unitOfWork;
        _paymentStrategy = paymentStrategy;
    }

    public Task Handle(PaymentConfirmation request, CancellationToken cancellationToken)
    {
        OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == request.Id);
        if (orderHeaderFromDb == null)
        {
            throw new NotFoundException("Order Header with given ID was not found in database");
        }
        var paymentStatus = _paymentStrategy.GetPaymentStatus(new StripeModel
        {
            SessionId = orderHeaderFromDb.SessionId
        });

        if (paymentStatus == "paid")
        {
            _unitOfWork.OrderHeader.UpdatePaymentID(request.Id, orderHeaderFromDb.SessionId, orderHeaderFromDb.PaymentIntendId);
            _unitOfWork.OrderHeader.UpdateStatus(request.Id, orderHeaderFromDb.OrderStatus, Constants.PaymentStatusApproved);
            _unitOfWork.Save();
        }

        return Task.CompletedTask;
    }
}
