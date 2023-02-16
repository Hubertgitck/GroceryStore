using Application.Models;
using ApplicationWeb.Mediator.Requests;
using Org.BouncyCastle.Tsp;

namespace ApplicationWeb.Mediator.Handlers;

public class AddCategoryHandler : IRequestHandler<AddCategory, string>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddCategoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<string> Handle(AddCategory request, CancellationToken cancellationToken)
    {
        _unitOfWork.Category.Add(request.Category);
        _unitOfWork.Save();

        return Task.FromResult("Index");
    }

}
