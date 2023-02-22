namespace ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

public record PaymentConfirmation : IRequest
{
    public readonly int Id;

    public PaymentConfirmation(int id)
    {
        Id = id;
    }
}
