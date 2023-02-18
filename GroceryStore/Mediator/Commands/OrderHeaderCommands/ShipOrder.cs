namespace ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

public record UpdateOrderHeader : IRequest<int>
{
    public OrderHeaderDto OrderHeaderDto;

    public UpdateOrderHeader(OrderHeaderDto orderHeaderDto)
    {
        OrderHeaderDto = orderHeaderDto;
    }
}