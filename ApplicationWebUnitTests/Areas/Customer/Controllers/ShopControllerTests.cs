using ApplicationWeb.Areas.Customer.Controllers;
using ApplicationWeb.Mediator.Commands.ShopCommands;
using ApplicationWeb.Mediator.Requests.ShopRequests;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class ShopControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ShopController _controller;

    public ShopControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ShopController(_mediatorMock.Object)
        {
            TempData = TempDataProvider.GetTempDataMock()     
        };
    }

    [Fact]
    public async Task Index_ShouldSendGetShopIndexViewRequest()
    {
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetShopIndexView>(), default));

        //Act
        var result = await _controller.Index(It.IsAny<string>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetShopIndexView>(), default), Times.Once);
    }

    [Fact]
    public async Task Details_ShouldSendGetProductDetailsRequest()
    {
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductDetails>(), default));

        //Act
        var result = await _controller.Details(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetProductDetails>(), default), Times.Once);
    }

    [Fact]
    public async Task Details_WithShoppingCartDto_ShouldSendDetailsPostRequest()
    {
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DetailsPost>(), default));

        //Act
        var result = await _controller.Details(It.IsAny<ShoppingCartDto>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<DetailsPost>(), default), Times.Once);
    }
}