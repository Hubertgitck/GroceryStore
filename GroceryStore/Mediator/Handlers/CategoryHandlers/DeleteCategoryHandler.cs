using ApplicationWeb.Mediator.Commands.CategoryCommands;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategory>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task Handle(DeleteCategory request, CancellationToken cancellationToken)
    {
        var categoryToDeleteInDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == request.Id);
        _unitOfWork.Category.Remove(categoryToDeleteInDb);
        _unitOfWork.Save();

        return Task.CompletedTask;   
    }
}
