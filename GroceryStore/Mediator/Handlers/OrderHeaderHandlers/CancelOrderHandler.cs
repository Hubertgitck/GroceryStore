using ApplicationWeb.Mediator.Commands.OrderHeaderCommands;


namespace ApplicationWeb.Mediator.Handlers.OrderHeaderHandlers;

public class CancelOrderHandler : IRequestHandler<CancelOrder>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StripeServiceProvider _stripeServices;

    public CancelOrderHandler(IUnitOfWork unitOfWork, StripeServiceProvider stripeServices)
    {
        _unitOfWork = unitOfWork;
        _stripeServices = stripeServices;
    }

    public Task Handle(CancelOrder request, CancellationToken cancellationToken)
    {
        var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(
            u => u.Id == request.Id, tracked: false);

        if (orderHeader.PaymentStatus == Constants.PaymentStatusApproved)
        {
            _stripeServices.GetRefundService(orderHeader.PaymentIntendId!);
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
