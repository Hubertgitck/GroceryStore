namespace ApplicationWeb.Mediator.DTO;

public class ShoppingCartViewDto
{
    public IEnumerable<ShoppingCartDto> CartList { get; set; }
    public OrderHeaderDto OrderHeaderDto { get; set; }
    public string PaymentDomain { get; set; }
}
