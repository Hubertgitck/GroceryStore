using ApplicationWeb.Mediator.Commands.ProductCommands;
using ApplicationWeb.Mediator.Requests.ProductRequests;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class ProductControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ProductController(_mediatorMock.Object)
        {
            TempData = TempDataProvider.GetTempDataMock()
        };
    }

    [Fact]
    public async Task Upsert_ShouldSendGetCategoryByGivenIdRequest()
    {
        //Act
        var result = await _controller.Upsert(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetProductViewById>(), default), Times.Once);
    }

    [Fact]
    public async Task Upsert_WithValidModel_ShouldSendUpsertRequest()
    {
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpsertProduct>(), default)).ReturnsAsync(It.IsAny<string>());

        //Act
        var result = await _controller.Upsert(GetTestProductViewDto(), null);

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<UpsertProduct>(), default), Times.Once);
    }

    [Fact]
    public async Task Upsert_WithInvalidModel_ShouldReturnViewWithThatModel()
    {
        //Arrange
        var productView = GetTestProductViewDto();
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        //Act
        var result = await _controller.Upsert(productView, null);

        //Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult!.Model.Should().BeEquivalentTo(productView);
    }

    [Fact]
    public async Task GetAll_ShouldSendGetAllProductsRequest()
    {
        //Act
        var result = await _controller.GetAll();

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetAllProducts>(), default), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldSendRequestToDeleteProductByGivenId()
    {
        //Act
        await _controller.Delete(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteProductById>(), default), Times.Once);
    }

    private ProductViewDto GetTestProductViewDto()
    {
        return new ProductViewDto { ProductDto = new ProductDto { Id = 1, Name = "Test" } };
    }
}