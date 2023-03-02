using ApplicationWeb.Areas.Admin.Controllers;
using ApplicationWeb.Mediator.Commands.CategoryCommands;
using ApplicationWeb.Mediator.Commands.OrderHeaderCommands;
using ApplicationWeb.Mediator.DTO;
using ApplicationWeb.Mediator.Requests.OrderDetailRequests;
using ApplicationWeb.Mediator.Requests.OrderHeaderRequests;
using ApplicationWebTests.TestUtilities;
using MediatR;

namespace ApplicationWebTests.Areas.Admin.Controllers;

public class OrderControllerTests
{

    private readonly Mock<IMediator> _mediatorMock;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new OrderController(_mediatorMock.Object)
        {
            TempData = TempDataProvider.GetTempDataMock()
        };
    }

    [Fact]
    public async Task Details_ShouldSendMediatorRequestsToCreateNewOrderDto()
    {
        //Act
        var result = await _controller.Details(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetOrderHeaderById>(), default), Times.Once);
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetAllOrderDetailsById>(), default), Times.Once);

        var viewResult = result as ViewResult;
        viewResult!.Model.Should().BeOfType<OrderDto>();
    }


    [Theory]
    [InlineData(1)]
    public async Task PaymentConfirmation_ShouldSendPaymentConfirmationRequest(int orderHeaderId)
    {
        //Act
        var result = await _controller.PaymentConfirmation(orderHeaderId);

        //Assert
        _mediatorMock.Verify(x => x.Send(It.Is<PaymentConfirmation>(r => r.Id == orderHeaderId), default), Times.Once);

        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(orderHeaderId);
    }

    [Fact]
    public async Task UpdateOrderDetail_ShoouldSendUpdateOrderHeaderRequest()
    {

    }


     private OrderHeader GetTestOrderHeader(int id)
     {

         var orderHeader = new OrderHeader
         {
             Id = id,
             Name = "Jane Doe",
             PhoneNumber = "0987654321",
             StreetAddress = "456 Main St",
             City = "New City",
             State = "New State",
             PostalCode = "54321",
             Carrier = "TestCarrier",
             TrackingNumber = "987654321",
             SessionId = "Test-session",
             PaymentIntendId = "Some-payment-IntendId"
         };
         return orderHeader;
     }
}