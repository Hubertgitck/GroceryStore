using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Requests.CategoryRequests;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryById, CategoryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCategoryByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<CategoryDto> Handle(GetCategoryById request, CancellationToken cancellationToken)
    {
        if (request.Id.GetValueOrDefault() == 0)
        {
            throw new ArgumentException("Invalid id");
        }

        var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == request.Id);

        if (categoryFromDb == null)
        {
            throw new NotFoundException($"Category with ID: {request.Id} was not found in database");
        }

        var categoryDto = _mapper.Map<CategoryDto>(categoryFromDb);

        return Task.FromResult(categoryDto);
    }
}
