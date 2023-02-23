namespace ApplicationWeb.Mediator.Requests.CartRequests;

public record IncrementCount : IRequest
{
	public readonly int Id;

	public IncrementCount(int id)
	{
		Id = id;
	}
}
