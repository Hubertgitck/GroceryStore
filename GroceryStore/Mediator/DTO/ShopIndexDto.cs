namespace ApplicationWeb.Mediator.DTO;

public class ShopIndexDto
{
	public IEnumerable<ProductDto> ProductsList { get; set; }
	public IEnumerable<CategoryDto> CategoryList { get; set; }
}
