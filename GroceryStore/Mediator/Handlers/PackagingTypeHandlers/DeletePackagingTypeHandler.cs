﻿using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Commands.PackagingTypeCommands;

namespace ApplicationWeb.Mediator.Handlers.PackagingTypeHandlers;

public class DeletePackagingTypeHandler : IRequestHandler<DeletePackagingType>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePackagingTypeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task Handle(DeletePackagingType request, CancellationToken cancellationToken)
    {
        var packagingTypeToDeleteInDb = _unitOfWork.PackagingType.GetFirstOrDefault(u => u.Id == request.Id);
        
        if(packagingTypeToDeleteInDb == null)
        {
            throw new NotFoundException("Packaging Type with given ID was not found in database");
        }

        _unitOfWork.PackagingType.Remove(packagingTypeToDeleteInDb);
        _unitOfWork.Save();

        return Task.CompletedTask;   
    }
}