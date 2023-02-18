namespace ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

public record ShipOrder : IRequest
{
    public OrderHeaderDto OrderHeaderDto;

    public ShipOrder(OrderHeaderDto orderHeaderDto)
    {
        OrderHeaderDto = orderHeaderDto;
    }
}