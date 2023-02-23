namespace ApplicationWeb.Mediator.Requests.CartRequests;

public record OrderConfirmation : IRequest
{
	public readonly int Id;

	public OrderConfirmation(int id)
	{
		Id = id;
	}
}
