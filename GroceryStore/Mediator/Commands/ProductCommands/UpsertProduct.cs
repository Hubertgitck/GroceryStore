namespace ApplicationWeb.Mediator.Commands.ProductCommands;

public record UpsertProduct : IRequest<string>
{
    public readonly ProductDto ProductDto;
    public readonly IFormFile? File;

    public UpsertProduct(ProductDto productDto, IFormFile? file)
    {
        ProductDto = productDto;
        File = file;
    }
}
