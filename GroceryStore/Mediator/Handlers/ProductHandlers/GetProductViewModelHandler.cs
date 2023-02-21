using Application.Models.ViewModels;
using ApplicationWeb.Mediator.Requests.ProductRequests;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers;

public class GetProductViewModelHandler : IRequestHandler<GetProductViewById, ProductViewDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetProductViewModelHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public Task<ProductViewDto> Handle(GetProductViewById request, CancellationToken cancellationToken)
    {
        ProductViewDto productViewDto = new()
        {
            ProductDto = new(),
            CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            PackagingTypeList = _unitOfWork.PackagingType
                .GetAll()
                .OrderBy(u => u.Weight)
                .Select(u => new SelectListItem
                {
                    Text = u.IsWeightInGrams == true ? u.Name +
                            $" {u.Weight * Constants.KilogramsToGramsFactor}[g]" : u.Name + $" {u.Weight}[kg]",
                    Value = u.Id.ToString(),
                })
        };

        if (request.Id == null || request.Id == 0)
        {
            return Task.FromResult(productViewDto);
        }
        else
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == request.Id);
            var productDto = _mapper.Map<ProductDto>(product);
            productViewDto.ProductDto = productDto;

            return Task.FromResult(productViewDto);
        }
    }
}
