using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Commands.CategoryCommands;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class DeleteCategoryByIdHandler : IRequestHandler<DeleteCategoryById>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task Handle(DeleteCategoryById request, CancellationToken cancellationToken)
    {
        var categoryToDeleteInDb = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == request.Id);

        if (categoryToDeleteInDb == null)
        {
            throw new NotFoundException($"Category with ID: {request.Id} was not found in database");
        }
        _unitOfWork.Category.Remove(categoryToDeleteInDb);
        _unitOfWork.Save();

        return Task.CompletedTask;   
    }
}
