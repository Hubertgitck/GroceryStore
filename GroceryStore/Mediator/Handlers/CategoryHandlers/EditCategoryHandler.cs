using ApplicationWeb.Mediator.Commands.CategoryCommands;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class EditCategoryHandler : IRequestHandler<EditCategory>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EditCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task Handle(EditCategory request, CancellationToken cancellationToken)
    {
        var categoryToEditInDb = _mapper.Map<Category>(request.CategoryDto);
        _unitOfWork.Category.Update(categoryToEditInDb);
        _unitOfWork.Save();

        return Task.CompletedTask;   
    }
}
