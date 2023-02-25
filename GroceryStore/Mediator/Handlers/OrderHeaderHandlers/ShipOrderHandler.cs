using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

namespace ApplicationWeb.Mediator.Handlers.OrderHeaderHandlers;

public class ShipOrderHandler : IRequestHandler<ShipOrder>
{
    private readonly IUnitOfWork _unitOfWork;

    public ShipOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task Handle(ShipOrder request, CancellationToken cancellationToken)
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(
            u => u.Id == request.OrderHeaderDto.Id, tracked: false);

        if (orderHeaderFromDb == null)
        {
            throw new NotFoundException($"Order Header with ID: {request.OrderHeaderDto.Id} was not found in database");
        }

        orderHeaderFromDb.TrackingNumber = request.OrderHeaderDto.TrackingNumber;
        orderHeaderFromDb.Carrier = request.OrderHeaderDto.Carrier;
        orderHeaderFromDb.OrderStatus = Constants.StatusShipped;
        orderHeaderFromDb.ShippingDate = DateTime.Now;

        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();

        return Task.CompletedTask;
    }
}
