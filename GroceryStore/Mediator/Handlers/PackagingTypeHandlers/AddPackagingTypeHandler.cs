using Application.Models;
using ApplicationWeb.Mediator.Commands.PackagingTypeCommands;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class AddCategoryHandler : IRequestHandler<AddPackagingType>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task Handle(AddPackagingType request, CancellationToken cancellationToken)
    {
        if (request.PackagingTypeDto.IsWeightInGrams)
        {
            request.PackagingTypeDto.Weight /= Constants.KilogramsToGramsFactor;
        }
        var packagingType = _mapper.Map<PackagingType>(request.PackagingTypeDto);
        _unitOfWork.PackagingType.Add(packagingType);
        _unitOfWork.Save();

        return Task.CompletedTask;  
    }
}
