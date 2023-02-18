using ApplicationWeb.Mediator.Requests.CategoryRequests;
using AutoMapper;

namespace ApplicationWeb.Mediator.Handlers.OrderDetailHandlers;

public class GetAllOrderDetailsByIdHandler : IRequestHandler<GetAllOrderDetailsById, IEnumerable<OrderDetailDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllOrderDetailsByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public Task<IEnumerable<OrderDetailDto>> Handle(GetAllOrderDetailsById request, CancellationToken cancellationToken)
    {
        var listOfOrderDetailsFromDb = _unitOfWork.OrderDetail
            .GetAll(u => u.OrderId == request.Id, includeProperties: "Product", thenIncludeProperty: "PackagingType");

        var result = _mapper.Map<IEnumerable<OrderDetailDto>>(listOfOrderDetailsFromDb);

        return Task.FromResult(result);
    }
}
