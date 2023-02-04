namespace Application.Models.ViewModels;

public class ShopIndexViewModel
{
    public IEnumerable<Product> ProductsList { get; set; }
    public IEnumerable<Category> CategoryList { get; set; }
}
