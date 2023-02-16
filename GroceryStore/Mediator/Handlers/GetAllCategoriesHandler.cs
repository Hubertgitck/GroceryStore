using ApplicationWeb.Mediator.Requests;

namespace ApplicationWeb.Mediator.Handlers;

public class GetAllCategoriesHandler : IRequestHandler<GetAllCategories, IEnumerable<Category>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllCategoriesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public Task<IEnumerable<Category>> Handle(GetAllCategories request, CancellationToken cancellationToken)
    {
        var result = _unitOfWork.Category.GetAll();
        return Task.FromResult(result);
    }
}
