namespace ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

public record PaymentConfirmation : IRequest
{
    public int Id;

    public PaymentConfirmation(int id)
    {
        Id = id;
    }
}
