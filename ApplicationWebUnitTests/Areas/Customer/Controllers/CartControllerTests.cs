using ApplicationWeb.Areas.Customer.Controllers;
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
        var productView = GetTestProductViewDto();
        //Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetSummaryView>(), default)).ReturnsAsync(productView);

        //Act
        var result = await _controller.Summary();

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetSummaryView>(), default), Times.Once);
    }

    /*    [Fact]
        public async Task SummaryPost_WhenCartListIsEmpty_ShouldRedirectToShopIndex()
        {
            //Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<SummaryPost>(), default)).ReturnsAsync("");

            //Act
            var result = await _controller.Index();

            //Assert
            _mediatorMock.Verify(x => x.Send(It.IsAny<GetCartIndexView>(), default), Times.Once);
        }*/
    private ShoppingCartViewDto GetTestProductViewDto()
    {
        return new ShoppingCartViewDto { OrderHeaderDto = new Mock<OrderHeaderDto>().Object };
    }
}