using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Commands.PackagingTypeCommands;

namespace ApplicationWeb.Mediator.Handlers.PackagingTypeHandlers;

public class DeletePackagingTypeHandler : IRequestHandler<DeletePackagingTypeById>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePackagingTypeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task Handle(DeletePackagingTypeById request, CancellationToken cancellationToken)
    {
        var packagingTypeToDeleteInDb = _unitOfWork.PackagingType.GetFirstOrDefault(u => u.Id == request.Id);
        
        if(packagingTypeToDeleteInDb == null)
        {
            throw new NotFoundException($"Order Header with ID: {request.Id} was not found in database");
        }

        _unitOfWork.PackagingType.Remove(packagingTypeToDeleteInDb);
        _unitOfWork.Save();

        return Task.CompletedTask;   
    }
}
