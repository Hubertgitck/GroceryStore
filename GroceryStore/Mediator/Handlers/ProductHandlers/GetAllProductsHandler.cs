using ApplicationWeb.Mediator.Requests.ProductRequests;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class GetAllProductsHandler : IRequestHandler<GetAllProducts, IEnumerable<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public Task<IEnumerable<ProductDto>> Handle(GetAllProducts request, CancellationToken cancellationToken)
    {
        var categoriesFromDb = _unitOfWork.Product.GetAll(includeProperties: "Category,PackagingType");
        var result = _mapper.Map<IEnumerable<ProductDto>>(categoriesFromDb);

        return Task.FromResult(result);
    }
}
