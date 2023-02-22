using ApplicationWeb.Mediator.Requests.OrderHeaderRequests;
using ApplicationWeb.Mediator.Utility;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class GetAllOrderHeadersHandler : IRequestHandler<GetAllOrderHeaders, IEnumerable<OrderHeaderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllOrderHeadersHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public Task<IEnumerable<OrderHeaderDto>> Handle(GetAllOrderHeaders request, CancellationToken cancellationToken)
    {
        IEnumerable<OrderHeader> orderHeadersFromDb;

        if (request.ClaimsPrincipal.IsInRole(Constants.RoleAdmin) || 
            request.ClaimsPrincipal.IsInRole(Constants.RoleEmployee))
        {
            orderHeadersFromDb = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
        }
        else
        {
            var userId = HelperMethods.GetApplicationUserIdFromClaimsPrincipal(request.ClaimsPrincipal);

            orderHeadersFromDb = _unitOfWork.OrderHeader.GetAll(
                u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
        }

        switch (request.Status)
        {
            case "inprocess":
                orderHeadersFromDb = orderHeadersFromDb.Where(u => u.OrderStatus == Constants.StatusInProcess);
                break;
            case "completed":
                orderHeadersFromDb = orderHeadersFromDb.Where(u => u.OrderStatus == Constants.StatusShipped);
                break;
            case "approved":
                orderHeadersFromDb = orderHeadersFromDb.Where(u => u.OrderStatus == Constants.StatusApproved);
                break;
            case "pending":
                orderHeadersFromDb = orderHeadersFromDb.Where(u => u.OrderStatus == Constants.StatusPending);
                break;
            default:
                break;
        }

        var result = _mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeadersFromDb);

        return Task.FromResult(result);
    }
}
