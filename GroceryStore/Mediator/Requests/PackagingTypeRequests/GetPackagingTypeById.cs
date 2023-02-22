namespace ApplicationWeb.Mediator.Requests.PackagingTypeRequests;

public record GetPackagingTypeById : IRequest<PackagingTypeDto>
{
    public readonly int? Id;

    public GetPackagingTypeById(int? id)
    {
        Id = id;
    }
}
