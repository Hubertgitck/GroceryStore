using ApplicationWeb.Mediator.Requests.CategoryRequests;
using AutoMapper;

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
        if (request.Id == null || request.Id == 0)
        {
            throw new Exception();
        }
        var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == request.Id);

        if (categoryFromDb == null)
        {
            throw new Exception();
        }

        var categoryDto = _mapper.Map<CategoryDto>(categoryFromDb);

        return Task.FromResult(categoryDto);
    }

}
