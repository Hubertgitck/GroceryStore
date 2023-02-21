namespace ApplicationWeb.Mediator.Commands.ProductCommands;

public record UpsertCommand : IRequest<string>
{
    public ProductDto ProductDto;
    public IFormFile? File;

    public UpsertCommand(ProductDto productDto, IFormFile? file)
    {
        ProductDto = productDto;
        File = file;
    }
}
