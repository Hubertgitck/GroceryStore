
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
        IEnumerable<PackagingType> packagingTypesFromDb = _unitOfWork.PackagingType.GetAll();

        foreach (var elem in packagingTypesFromDb)
        {
            if (elem.IsWeightInGrams)
            {
                elem.Weight *= Constants.KilogramsToGramsFactor;
            }
        }
        var result = _mapper.Map<IEnumerable<PackagingTypeDto>>(packagingTypesFromDb);

        return Task.FromResult(result);
    }
}
