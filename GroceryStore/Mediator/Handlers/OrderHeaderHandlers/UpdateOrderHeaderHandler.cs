using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class UpdateOrderHeaderHandler : IRequestHandler<UpdateOrderHeader, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderHeaderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<int> Handle(UpdateOrderHeader request, CancellationToken cancellationToken)
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(
            u => u.Id == request.OrderHeaderDto.Id, tracked: false);

        if (orderHeaderFromDb == null)
        {
            throw new NotFoundException($"Order Header with ID: {request.OrderHeaderDto.Idd} was not found in database");
        }

        orderHeaderFromDb.Name = request.OrderHeaderDto.Name;
        orderHeaderFromDb.PhoneNumber = request.OrderHeaderDto.PhoneNumber;
        orderHeaderFromDb.StreetAddress = request.OrderHeaderDto.StreetAddress;
        orderHeaderFromDb.City = request.OrderHeaderDto.City;
        orderHeaderFromDb.State = request.OrderHeaderDto.State;
        orderHeaderFromDb.PostalCode = request.OrderHeaderDto.PostalCode;

        if (request.OrderHeaderDto.Carrier != null)
        {
            orderHeaderFromDb.Carrier = request.OrderHeaderDto.Carrier;
        }
        if (request.OrderHeaderDto.TrackingNumber != null)
        {
            orderHeaderFromDb.TrackingNumber = request.OrderHeaderDto.TrackingNumber;
        }
        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();

        return Task.FromResult(orderHeaderFromDb.Id);
    }
}
