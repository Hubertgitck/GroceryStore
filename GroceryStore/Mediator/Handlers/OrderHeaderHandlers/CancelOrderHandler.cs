using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Commands.OrderHeaderCommands;
using ApplicationWeb.Payments.Models;
using ApplicationWeb.PaymentServices.Interfaces;

namespace ApplicationWeb.Mediator.Handlers.OrderHeaderHandlers;

public class CancelOrderHandler : IRequestHandler<CancelOrder>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentStrategy _paymentStrategy;

    public CancelOrderHandler(IUnitOfWork unitOfWork,  IPaymentStrategy paymentStrategy)
    {
        _unitOfWork = unitOfWork;
        _paymentStrategy = paymentStrategy;
    }

    public Task Handle(CancelOrder request, CancellationToken cancellationToken)
    {
        var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(
            u => u.Id == request.Id, tracked: false);

        if (orderHeader == null)
        {
            throw new NotFoundException("Order Header with given ID was not found in database");
        }

        if (orderHeader.PaymentStatus == Constants.PaymentStatusApproved)
        {
            _paymentStrategy.MakeRefund(new StripeModel { PaymentIntentId = orderHeader.PaymentIntendId });
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, Constants.StatusCancelled, Constants.StatusRefunded);
        }
        else
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, Constants.StatusCancelled, Constants.StatusCancelled);
        }

        _unitOfWork.Save();

        return Task.CompletedTask;
    }
}
