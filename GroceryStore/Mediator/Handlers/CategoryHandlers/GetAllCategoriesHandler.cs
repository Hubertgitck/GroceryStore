using ApplicationWeb.Mediator.Requests.CategoryRequests;
using AutoMapper;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class GetAllCategoriesHandler : IRequestHandler<GetAllCategories, IEnumerable<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCategoriesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public Task<IEnumerable<CategoryDto>> Handle(GetAllCategories request, CancellationToken cancellationToken)
    {
        var categoriesFromDb = _unitOfWork.Category.GetAll();
        var result = _mapper.Map<IEnumerable<CategoryDto>>(categoriesFromDb);

        return Task.FromResult(result);
    }
}
