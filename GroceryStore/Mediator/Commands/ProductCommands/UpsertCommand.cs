namespace ApplicationWeb.Mediator.Commands.ProductCommands;

public record UpsertCommand : IRequest<string>
{
    public ProductViewDto ProductViewDto;
    public IFormFile? File;

    public UpsertCommand(ProductViewDto productViewDto, IFormFile? file)
    {
        ProductViewDto = productViewDto;
        File = file;
    }
}
