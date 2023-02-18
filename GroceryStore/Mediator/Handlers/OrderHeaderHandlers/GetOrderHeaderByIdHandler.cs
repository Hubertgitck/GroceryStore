using ApplicationWeb.Mediator.Requests.OrderHeaderRequests;
using AutoMapper;

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
            throw new Exception();
        }
        var orderHeaderFromDb = _unitOfWork.OrderHeader
            .GetFirstOrDefault(u => u.Id == request.Id, includeProperties: "ApplicationUser");

        var orderHeaderDto = _mapper.Map<OrderHeaderDto>(orderHeaderFromDb);

        return Task.FromResult(orderHeaderDto);
    }
}
