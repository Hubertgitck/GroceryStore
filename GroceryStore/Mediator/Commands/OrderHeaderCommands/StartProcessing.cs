namespace ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

public record StartProcessing : IRequest
{
    public readonly int Id;

    public StartProcessing(int id)
    {
        Id = id;
    }
}