using ApplicationWeb.Mediator.Requests.ShopRequests;

namespace ApplicationWeb.Mediator.Handlers.ShopHandlers;

public class GetShopIndexViewHandler : IRequestHandler<GetShopIndexView, ShopIndexDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetShopIndexViewHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<ShopIndexDto> Handle(GetShopIndexView request, CancellationToken cancellationToken)
    {
        IEnumerable<Product> productsCollectionFromDb;

        if (string.IsNullOrEmpty(request.Category))
        {
            productsCollectionFromDb = _unitOfWork.Product.GetAll(includeProperties: "Category,PackagingType")
                .OrderBy(u => u.Category.DisplayOrder);
        }
        else
        {
            productsCollectionFromDb = _unitOfWork.Product.GetAll(u => u.Category.Name == request.Category,
               includeProperties: "Category,PackagingType")
                .OrderBy(u => u.Category.DisplayOrder);
        }

        ShopIndexDto shopIndexDto = new()
        {
            ProductsList = _mapper.Map<IEnumerable<ProductDto>>(productsCollectionFromDb),
            CategoryList = _mapper.Map<IEnumerable<CategoryDto>>(_unitOfWork.Category.GetAll())
        };

        return await Task.FromResult(shopIndexDto);
    }
}
