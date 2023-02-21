using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Commands.OrderHeaderCommands;
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
        OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == request.Id);
        if (orderHeaderFromDb == null)
        {
            throw new NotFoundException("Order Header with given ID was not found in database");
        }
        Session session = _stripeServices.GetStripeSession(orderHeaderFromDb.SessionId);

        if (session.PaymentStatus.ToLower() == "paid")
        {
            _unitOfWork.OrderHeader.UpdateStripePaymentID(request.Id, orderHeaderFromDb.SessionId, session.PaymentIntentId);
            _unitOfWork.OrderHeader.UpdateStatus(request.Id, orderHeaderFromDb.OrderStatus, Constants.PaymentStatusApproved);
            _unitOfWork.Save();
        }

        return Task.CompletedTask;
    }
}
