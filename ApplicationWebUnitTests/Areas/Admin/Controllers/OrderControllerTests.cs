using Application.Models.ViewModels;
using Application.Models;
using Application.Utility;
using ApplicationWeb.Areas.Admin.Controllers;
using Stripe.Checkout;
using ApplicationWebTests.TestUtilities;

namespace ApplicationWebTests.Areas.Admin.Controllers;

public class OrderControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<StripeServiceProvider> _stripeServiceMock = new(MockBehavior.Loose);
    private readonly ITempDataDictionary _tempData;

    public OrderControllerTests()
    {
        _tempData = TempDataProvider.GetTempDataMock();
    }

    [Theory]
    [InlineData(1)]
    public void PaymentConfirmation_UpdatesOrderHeader_WhenPaymentIsPaid(int orderHeaderId)
    {
        //Arrange
        var orderHeader = GetTestOrderHeader(orderHeaderId);

        var session = new Session { PaymentStatus = "paid", PaymentIntentId = "some-payment-intent-id" };

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), true))
            .Returns(orderHeader);
        _unitOfWorkMock.Setup(u => u.OrderHeader.UpdateStripePaymentID(orderHeaderId, It.IsAny<string>(), It.IsAny<string>()));
        _unitOfWorkMock.Setup(u => u.OrderHeader.UpdateStatus(orderHeaderId, It.IsAny<string>(), Constants.PaymentStatusApproved));
        _unitOfWorkMock.Setup(u => u.Save());

        var stripeServices = new Mock<StripeServiceProvider>();
        _stripeServiceMock.Setup(u => u.GetStripeSession(orderHeader.SessionId!)).Returns(session);

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);

        //Act
        var result = controller.PaymentConfirmation(orderHeaderId);

        //Assert
        _stripeServiceMock.Verify(s => s.GetStripeSession(orderHeader.SessionId!), Times.Once());
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStripePaymentID(orderHeaderId, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStatus(orderHeaderId, It.IsAny<string>(), Constants.PaymentStatusApproved), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Theory]
    [InlineData(1)]
    public void Update_WhenCarrierIsNotNull_UpdatesCarrierInOrderHeaderFromDb(int orderHeaderId)
    {
        //Arrange
        var orderHeaderFromDb = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderViewModel.OrderHeader = new()
        {
            Carrier = "TestCarrier"
        };

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), false))
            .Returns(orderHeaderFromDb);

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);
        controller.TempData = _tempData;

        //Act
        var result = controller.UpdateOrderDetail(orderViewModel);

        //Assert
        orderHeaderFromDb.Carrier.Should().Be(orderViewModel.OrderHeader.Carrier);
    }

    [Theory]
    [InlineData(1)]
    public void Update_WhenTrackingNumberIsNotNull_UpdatesTrackingNumberInOrderHeaderFromDb(int orderHeaderId)
    {
        //Arrange
        var orderHeaderFromDb = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderViewModel.OrderHeader = new()
        {
            TrackingNumber = "TestTrackingNumber"
        };

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), false))
            .Returns(orderHeaderFromDb);

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);
        controller.TempData = _tempData;

        //Act
        var result = controller.UpdateOrderDetail(orderViewModel);

        //Assert
        orderHeaderFromDb.TrackingNumber.Should().Be(orderViewModel.OrderHeader.TrackingNumber);
    }

    [Theory]
    [InlineData(1, Constants.PaymentStatusApproved)]
    public void CancelOrder_WithPaymentStatusApproved_RefundsPaymentAndUpdatesStatus(int orderHeaderId, string status)
    {
        //Arrange
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderHeader.PaymentStatus = status;
        orderViewModel.OrderHeader = new();

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), false))
            .Returns(orderHeader);
        _stripeServiceMock.Setup(u => u.GetRefundService(orderHeader.PaymentIntendId!));

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);
        controller.TempData = _tempData;

        //Act
        var result = controller.CancelOrder(orderViewModel);

        //Assert
        _stripeServiceMock.Verify(s => s.GetRefundService(orderHeader.PaymentIntendId!), Times.Once());
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStatus(orderHeaderId, Constants.StatusCancelled, It.IsAny<string>()), Times.Once());
    }

    [Theory]
    [InlineData(1, Constants.PaymentStatusPending)]
    [InlineData(2, Constants.PaymentStatusRejected)]
    public void CancelOrder_WithPaymentStatusOtherThanApproved_JustUpdatesStatus(int orderHeaderId, string status)
    {
        //Arrange
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderHeader.PaymentStatus = status;
        orderViewModel.OrderHeader = new();

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), false))
            .Returns(orderHeader);

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);
        controller.TempData = _tempData;

        //Act
        var result = controller.CancelOrder(orderViewModel);

        //Assert
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStatus(orderHeaderId, Constants.StatusCancelled, It.IsAny<string>()), Times.Once());
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