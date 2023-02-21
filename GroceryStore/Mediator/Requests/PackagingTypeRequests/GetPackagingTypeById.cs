namespace ApplicationWeb.Mediator.Requests.PackagingTypeRequests;

public record GetPackagingTypeById : IRequest<PackagingTypeDto>
{
    public int? Id;

    public GetPackagingTypeById(int? id)
    {
        Id = id;
    }
}
