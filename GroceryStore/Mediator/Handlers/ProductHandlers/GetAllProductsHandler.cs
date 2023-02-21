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
        var categoriesCollectionFromDb = _unitOfWork.Product.GetAll(includeProperties: "Category,PackagingType");
        var categoriesCollectionDto = _mapper.Map<IEnumerable<ProductDto>>(categoriesCollectionFromDb);

        return Task.FromResult(categoriesCollectionDto);
    }
}
