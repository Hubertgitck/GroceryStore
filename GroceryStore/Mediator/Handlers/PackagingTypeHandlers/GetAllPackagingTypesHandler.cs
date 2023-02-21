using ApplicationWeb.Mediator.Requests.PackagingTypeRequests;

namespace ApplicationWeb.Mediator.Handlers.PackagingTypeHandlers;

public class GetAllPackagingTypesHandler : IRequestHandler<GetAllPackagingTypes, IEnumerable<PackagingTypeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPackagingTypesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public Task<IEnumerable<PackagingTypeDto>> Handle(GetAllPackagingTypes request, CancellationToken cancellationToken)
    {
        var packagingTypesCollectionFromDb = _unitOfWork.PackagingType.GetAll();

        foreach (var packagingType in packagingTypesCollectionFromDb)
        {
            if (packagingType.IsWeightInGrams)
            {
                packagingType.Weight *= Constants.KilogramsToGramsFactor;
            }
        }
        var packagingTypesCollectionDto = _mapper.Map<IEnumerable<PackagingTypeDto>>(packagingTypesCollectionFromDb);

        return Task.FromResult(packagingTypesCollectionDto);
    }
}
