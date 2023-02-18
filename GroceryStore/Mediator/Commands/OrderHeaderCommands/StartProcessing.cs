namespace ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

public record StartProcessing : IRequest
{
    public int Id;

    public StartProcessing(int id)
    {
        Id = id;
    }
}