namespace ApplicationWeb.Mediator.Commands.ProductCommands;

public record Upsert : IRequest<string>
{
    public readonly ProductDto ProductDto;
    public readonly IFormFile? File;

    public Upsert(ProductDto productDto, IFormFile? file)
    {
        ProductDto = productDto;
        File = file;
    }
}
