using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApplicationWeb.Mediator.DTO;

public class ProductViewDto
{
    public ProductDto ProductDto { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem>? CategoryList { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem>? PackagingTypeList { get; set; }
}
