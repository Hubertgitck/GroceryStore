namespace ApplicationWeb.Mediator.Requests.OrderHeaderRequests
{
    public class GetOrderHeaderById : IRequest<OrderHeaderDto>
    {
        public int Id;

        public GetOrderHeaderById(int id)
        {
            Id = id;
        }
    }
}
