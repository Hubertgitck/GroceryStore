using ApplicationWeb.Areas.Customer.Controllers;
using ApplicationWeb.Mediator.Commands.CartCommands;
using ApplicationWeb.Mediator.Requests.CartRequests;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class CartControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CartController _controller;

    public CartControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CartController(_mediatorMock.Object)
        {
            TempData = TempDataProvider.GetTempDataMock()     
        };
    }

    [Fact]
    public async Task Index_ShouldSendGetCartIndexViewRequest()
    {
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartIndexView>(), default));

        //Act
        var result = await _controller.Index();

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetCartIndexView>(), default), Times.Once);
    }

    [Fact]
    public async Task Summary_ShouldSendGetSummaryViewRequest()
    {
        var productView = GetTestProductViewDtoWithoutCarts();
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetSummaryView>(), default)).ReturnsAsync(productView);

        //Act
        var result = await _controller.Summary();

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetSummaryView>(), default), Times.Once);
    }

    [Fact]
    public async Task Summary_WhenCartListIsEmpty_ShouldRedirectToShopIndex()
    {
        //Arrange
        var shoppingCartView = GetTestProductViewDtoWithoutCarts();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetSummaryView>(), default)).ReturnsAsync(shoppingCartView);

        //Act
        var result = await _controller.Summary();

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetSummaryView>(), default), Times.Once);
        result.Should().BeOfType(typeof(RedirectToActionResult));
    } 
    
    [Fact]
    public async Task Summary_WhenCartListIsNotEmpty_ShouldReturnShoppingCartView()
    {
        //Arrange
        var shoppingCartView = GetTestProductViewDtoWithCarts();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetSummaryView>(), default)).ReturnsAsync(shoppingCartView);

        //Act
        var result = await _controller.Summary();

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetSummaryView>(), default), Times.Once);
        result.Should().BeOfType(typeof(ViewResult));
    }

    [Fact]
    public async Task SummaryPost_ShouldSendSummaryPostRequest()
    {
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<SummaryPost>(), default));

        //Act
        var result = await _controller.SummaryPost(It.IsAny<ShoppingCartViewDto>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<SummaryPost>(), default), Times.Once);
    }

    [Fact]
    public async Task SummaryPost_WhenResultIsEmpty_ShouldRedirectToShopIndex()
    {
        //Arrange
        var shoppingCartView = GetTestProductViewDtoWithoutCarts();
        _mediatorMock.Setup(m => m.Send(It.IsAny<SummaryPost>(), default)).ReturnsAsync("");

        //Act
        var result = await _controller.SummaryPost(It.IsAny<ShoppingCartViewDto>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<SummaryPost>(), default), Times.Once);
        result.Should().BeOfType(typeof(RedirectToActionResult));
    }

    [Fact]
    public async Task SummaryPost_WhenResultIsNotEmpty_ShouldRedirectToPaymentWebsite()
    {
        //Arrange
        var shoppingCartView = GetTestProductViewDtoWithCarts();
        _mediatorMock.Setup(m => m.Send(It.IsAny<SummaryPost>(), default)).ReturnsAsync("testRedirect");
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();

        //Act
        var result = await _controller.SummaryPost(It.IsAny<ShoppingCartViewDto>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<SummaryPost>(), default), Times.Once);
        result.Should().BeOfType(typeof(StatusCodeResult));
    }

    [Fact]
    public async Task OrderConfirmation_ShouldSendOrderConfirmationRequest()
    {
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<OrderConfirmation>(), default));

        var httpContext = new DefaultHttpContext() 
        { 
            Session = new Mock<ISession>().Object,
        };
        _controller.ControllerContext.HttpContext = httpContext;

        //Act
        var result = await _controller.OrderConfirmation(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<OrderConfirmation>(), default), Times.Once);
    }

    [Fact]
    public async Task IncrementCount_ShouldSendIncrementCountRequest()
    {
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<IncrementCount>(), default));

        //Act
        var result = await _controller.IncrementCount(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<IncrementCount>(), default), Times.Once);
    }

    [Fact]
    public async Task DecrementCount_ShouldSendDecrementCountRequest()
    {
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DecrementCount>(), default));

        //Act
        var result = await _controller.DecrementCount(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<DecrementCount>(), default), Times.Once);
    }

    [Fact]
    public async Task Remove_ShouldSendRemoveCartByIdRequest()
    {
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveCartById>(), default));

        //Act
        var result = await _controller.Remove(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<RemoveCartById>(), default), Times.Once);
    }

    private ShoppingCartViewDto GetTestProductViewDtoWithoutCarts()
    {
        return new ShoppingCartViewDto { CartList = new List<ShoppingCartDto>(), OrderHeaderDto = new Mock<OrderHeaderDto>().Object };
    }    
    private ShoppingCartViewDto GetTestProductViewDtoWithCarts()
    {
        return new ShoppingCartViewDto { 
            CartList = new List<ShoppingCartDto> 
            { 
                new ShoppingCartDto { Id = 1},
                new ShoppingCartDto { Id = 2}
            } , 
            OrderHeaderDto = new Mock<OrderHeaderDto>().Object };
    }
}