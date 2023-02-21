namespace ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

public record CancelOrder : IRequest
{
    public readonly int Id;
    public CancelOrder(int id)
    {
        Id = id;
    }
}
