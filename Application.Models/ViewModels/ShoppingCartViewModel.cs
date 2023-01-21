using Application.Models;

namespace Application.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> CartList { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
