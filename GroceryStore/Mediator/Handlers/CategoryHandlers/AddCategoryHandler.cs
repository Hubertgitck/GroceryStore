using ApplicationWeb.Mediator.Commands.CategoryCommands;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class AddPackagingTypeHandler : IRequestHandler<AddCategory>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddPackagingTypeHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task Handle(AddCategory request, CancellationToken cancellationToken)
    {
        var categoryToAddToDb = _mapper.Map<Category>(request.CategoryDto);
        _unitOfWork.Category.Add(categoryToAddToDb);
        _unitOfWork.Save();
        
        return Task.CompletedTask;  
    }
}
