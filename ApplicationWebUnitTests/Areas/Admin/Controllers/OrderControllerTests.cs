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
    private readonly OrderHeaderDto _testOrderHeaderDto;

    public OrderControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new OrderController(_mediatorMock.Object)
        {
            TempData = TempDataProvider.GetTempDataMock()
        };
        _testOrderHeaderDto = GetTestOrderHeader();
    }

    [Fact]
    public async Task Details_ShouldSendRequestsToCreateNewOrderDto()
    {
        //Act
        var result = await _controller.Details(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetOrderHeaderById>(), default), Times.Once);
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetAllOrderDetailsById>(), default), Times.Once);

        var viewResult = result as ViewResult;
        viewResult!.Model.Should().BeOfType<OrderDto>();
    }


    [Fact]
    public async Task PaymentConfirmation_ShouldSendPaymentConfirmationRequest()
    {
        //Arrange
        var orderHeaderId = _testOrderHeaderDto.Id;
        //Act
        var result = await _controller.PaymentConfirmation(orderHeaderId);

        //Assert
        _mediatorMock.Verify(x => x.Send(It.Is<PaymentConfirmation>(r => r.Id == orderHeaderId), default), Times.Once);

        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(orderHeaderId);
    }

    [Fact]
    public async Task UpdateOrderDetail_ShouldSendUpdateOrderHeaderRequest()
    {
        //Act
        await _controller.UpdateOrderDetail(It.IsAny<OrderHeaderDto>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateOrderHeader>(), default), Times.Once);
    }

    [Fact]
    public async Task StartProcessing_ShouldSendUpdateStartProcessingRequest()
    {
        //Act
        await _controller.StartProcessing(_testOrderHeaderDto);

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<StartProcessing>(), default), Times.Once);
    }

    [Fact]
    public async Task ShipOrder_ShouldSendShipOrderRequest()
    {
        //Act
        await _controller.ShipOrder(_testOrderHeaderDto);

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<ShipOrder>(), default), Times.Once);
    }

    [Fact]
    public async Task CancelOrder_ShouldSendCancelOrderRequest()
    {
        //Act
        var result = await _controller.CancelOrder(_testOrderHeaderDto);

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<CancelOrder>(), default), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldSendGetAllOrderHeadersRequest()
    {
        //Act
        var result = await _controller.GetAll(It.IsAny<string>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetAllOrderHeaders>(), default), Times.Once);
    }

    private OrderHeaderDto GetTestOrderHeader()
     {

         var orderHeaderDto = new OrderHeaderDto
         {
             Id = 1,
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
         return orderHeaderDto;
     }
}