namespace ApplicationWeb.Mediator.DTO;

public class OrderDetailDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public OrderHeaderDto OrderHeaderDto { get; set; }

    public int ProductId { get; set; }
    public ProductDto ProductDto { get; set; }
    public int Count { get; set; }
    public double Price { get; set; }
}
