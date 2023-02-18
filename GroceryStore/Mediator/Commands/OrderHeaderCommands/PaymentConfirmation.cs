namespace ApplicationWeb.Mediator.Commands.OrderHeaderCommands;

public class PaymentConfirmation : IRequest
{
    public int Id;

    public PaymentConfirmation(int id)
    {
        Id = id;
    }
}
