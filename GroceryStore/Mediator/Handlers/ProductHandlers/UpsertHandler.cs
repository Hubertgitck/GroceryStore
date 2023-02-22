using ApplicationWeb.Mediator.Commands.ProductCommands;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class UpsertHandler : IRequestHandler<Upsert, string>
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
    public Task<string> Handle(Upsert request, CancellationToken cancellationToken)
    {
        string message;
        string wwwRootPath = _hostEnvironment.WebRootPath;

        if (request.File != null)
        {
            string fileName = Guid.NewGuid().ToString();
            var uploads = Path.Combine(wwwRootPath, @"img\products");
            var extension = Path.GetExtension(request.File.FileName);

            if (request.ProductDto.ImageUrl != null)
            {
                var oldImagePath = Path.Combine(wwwRootPath, request.ProductDto.ImageUrl.TrimStart('\\'));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                request.File.CopyTo(fileStreams);
            }
            request.ProductDto.ImageUrl = @"\img\products\" + fileName + extension;
        }

        var productDb = _mapper.Map<Product>(request.ProductDto);

        if (request.ProductDto.Id == 0)
        {
            _unitOfWork.Product.Add(productDb);
            message = "created";
        }
        else
        {
            _unitOfWork.Product.Update(productDb);
            message = "updated";
        }
        _unitOfWork.Save();

        return Task.FromResult(message);
    }
}
