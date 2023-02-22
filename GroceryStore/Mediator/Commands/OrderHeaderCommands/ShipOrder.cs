namespace ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

public record UpdateOrderHeader : IRequest<int>
{
    public readonly OrderHeaderDto OrderHeaderDto;

    public UpdateOrderHeader(OrderHeaderDto orderHeaderDto)
    {
        OrderHeaderDto = orderHeaderDto;
    }
}