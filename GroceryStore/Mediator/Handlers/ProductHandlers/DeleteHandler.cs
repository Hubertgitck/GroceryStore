using ApplicationWeb.Mediator.Commands.ProductCommands;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class GetProductByIdHandler : IRequestHandler<DeleteCommand, Dictionary<string, string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;

    public GetProductByIdHandler(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
    }

    public Task<Dictionary<string,string>> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        Dictionary<string, string> dictionary;

        var product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == request.Id);
        if (product == null)
        {
            dictionary = new()
            {
                { "success", "false" },
                { "message", "Error while deleting" }
            };
            return Task.FromResult(dictionary);
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));  
        if (File.Exists(oldImagePath))
        {
            File.Delete(oldImagePath);
        }

        _unitOfWork.Product.Remove(product);
        _unitOfWork.Save();

        dictionary = new()
            {
                { "success", "true" },
                { "message", "Product deleted successfully" }
            };

        return Task.FromResult(dictionary);
    }
}
