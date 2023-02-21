using Application.Utility.Exceptions;
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
        if (categoryToDeleteInDb == null)
        {
            throw new NotFoundException("Category with given ID was not found in database");
        }
        _unitOfWork.Category.Remove(categoryToDeleteInDb);
        _unitOfWork.Save();

        return Task.CompletedTask;   
    }
}
