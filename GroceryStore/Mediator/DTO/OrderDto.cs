namespace ApplicationWeb.Mediator.DTO
{
    public class OrderDto
    {
        public OrderHeaderDto OrderHeaderDto { get; set; }
        public IEnumerable<OrderDetailDto> OrderDetailDtos { get; set; }
    }
}