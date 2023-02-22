namespace ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

public record ShipOrder : IRequest
{
    public readonly OrderHeaderDto OrderHeaderDto;

    public ShipOrder(OrderHeaderDto orderHeaderDto)
    {
        OrderHeaderDto = orderHeaderDto;
    }
}