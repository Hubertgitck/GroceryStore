using ApplicationWeb.Mediator.Commands.PackagingTypeCommands;
using ApplicationWeb.Mediator.DTO;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class EditPackagingTypeHandler : IRequestHandler<EditPackagingType>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EditPackagingTypeHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task Handle(EditPackagingType request, CancellationToken cancellationToken)
    {
        if (request.PackagingTypeDto.IsWeightInGrams)
        {
            request.PackagingTypeDto.Weight /= Constants.KilogramsToGramsFactor;
        }

        var packagingType = _mapper.Map<PackagingType>(request.PackagingTypeDto);

        _unitOfWork.PackagingType.Update(packagingType);
        _unitOfWork.Save();

        return Task.CompletedTask;   
    }
}
