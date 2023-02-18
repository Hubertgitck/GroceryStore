using ApplicationWeb.Mediator.Commands.CategoryCommands;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class AddCategoryHandler : IRequestHandler<AddCategory>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task Handle(AddCategory request, CancellationToken cancellationToken)
    {
        var categoryToAddToDb = _mapper.Map<Category>(request.Category);
        _unitOfWork.Category.Add(categoryToAddToDb);
        _unitOfWork.Save();
        
        return Task.CompletedTask;  
    }
}
