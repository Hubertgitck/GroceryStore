using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Requests.PackagingTypeRequests;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class GetPackagingTypeByIdHandler : IRequestHandler<GetPackagingTypeById, PackagingTypeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPackagingTypeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<PackagingTypeDto> Handle(GetPackagingTypeById request, CancellationToken cancellationToken)
    {
        if (request.Id.GetValueOrDefault() == 0)
        {
            throw new ArgumentException("Invalid id");
        }
        var packagingTypeFromDb = _unitOfWork.PackagingType.GetFirstOrDefault(c => c.Id == request.Id);

        if (packagingTypeFromDb == null)
        {
            throw new NotFoundException($"PackagingType with ID: {request.Id} was not found in database");
        }

        if (packagingTypeFromDb.IsWeightInGrams)
        {
            packagingTypeFromDb.Weight *= Constants.KilogramsToGramsFactor;
        }

        var packagingTypeDto = _mapper.Map<PackagingTypeDto>(packagingTypeFromDb);

        return Task.FromResult(packagingTypeDto);
    }
}
