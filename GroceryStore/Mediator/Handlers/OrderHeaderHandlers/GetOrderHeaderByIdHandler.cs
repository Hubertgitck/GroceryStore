using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Requests.OrderHeaderRequests;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class GetOrderHeaderByIdHandler : IRequestHandler<GetOrderHeaderById, OrderHeaderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderHeaderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<OrderHeaderDto> Handle(GetOrderHeaderById request, CancellationToken cancellationToken)
    {
        if (request.Id == 0)
        {
            throw new ArgumentException("Invalid id");
        }

        var orderHeaderFromDb = _unitOfWork.OrderHeader
            .GetFirstOrDefault(u => u.Id == request.Id, includeProperties: "ApplicationUser");

        if (orderHeaderFromDb == null)
        {
            throw new NotFoundException("Order Header with given ID was not found in database");
        }

        var orderHeaderDto = _mapper.Map<OrderHeaderDto>(orderHeaderFromDb);

        return Task.FromResult(orderHeaderDto);
    }
}
