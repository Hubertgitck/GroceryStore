namespace ApplicationWeb.Mediator.Commands.ProductCommands;

public record UpsertCommand : IRequest<string>
{
    public readonly ProductDto ProductDto;
    public readonly IFormFile? File;

    public UpsertCommand(ProductDto productDto, IFormFile? file)
    {
        ProductDto = productDto;
        File = file;
    }
}
