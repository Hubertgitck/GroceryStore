namespace ApplicationWeb.Mediator.Commands.OrderHeaderCommands
{
    public class CancelOrder : IRequest
    {
        public readonly int Id;
        public CancelOrder(int id)
        {
            Id = id;
        }
    }
}
