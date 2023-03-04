using ApplicationWeb.Areas.Customer.Controllers;

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



}