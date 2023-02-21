using ApplicationWeb.Mediator.Commands.ProductCommands;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class UpsertHandler : IRequestHandler<UpsertCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _hostEnvironment;

    public UpsertHandler(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _hostEnvironment = hostEnvironment;
    }
    public Task<string> Handle(UpsertCommand request, CancellationToken cancellationToken)
    {
        string message;
        string wwwRootPath = _hostEnvironment.WebRootPath;
        if (request.File != null)
        {
            string fileName = Guid.NewGuid().ToString();
            var uploads = Path.Combine(wwwRootPath, @"img\products");
            var extension = Path.GetExtension(request.File.FileName);

            if (request.ProductViewDto.ProductDto.ImageUrl != null)
            {
                var oldImagePath = Path.Combine(wwwRootPath, request.ProductViewDto.ProductDto.ImageUrl.TrimStart('\\'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                request.File.CopyTo(fileStreams);
            }
            request.ProductViewDto.ProductDto.ImageUrl = @"\img\products\" + fileName + extension;
        }

        var product = _mapper.Map<Product>(request.ProductViewDto.ProductDto);

        if (request.ProductViewDto.ProductDto.Id == 0)
        {
            _unitOfWork.Product.Add(product);
            message = "created";
        }
        else
        {
            _unitOfWork.Product.Update(product);
            message = "updated";
        }

        _unitOfWork.Save();
        return Task.FromResult(message);
    }
}
