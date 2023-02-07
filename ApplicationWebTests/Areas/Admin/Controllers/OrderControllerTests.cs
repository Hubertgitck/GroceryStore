using Application.Models;
using Application.Models.ViewModels;
using Application.Utility;
using Microsoft.AspNetCore.Authorization;
using Stripe;
using Stripe.Checkout;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class OrderControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ITempDataProvider _tempDataProvider;
    private readonly TempDataDictionaryFactory _tempDataDictionaryFactory;
    private readonly ITempDataDictionary _tempData;

    public OrderControllerTests()
    {
        _tempDataProvider = Mock.Of<ITempDataProvider>();
        _tempDataDictionaryFactory = new TempDataDictionaryFactory(_tempDataProvider);
        _tempData = _tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
    }

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Arrange
        var controller = new OrderController(_unitOfWorkMock.Object, null);

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        viewResult.Should().BeOfType<ViewResult>();
    }

    [Theory]
    [InlineData(1)]
    public void Details_ReturnsViewResult_WithCorrectOrderViewModel(int orderId)
    {
        // Arrange
        var orderHeader = GetTestOrderHeader(orderId);
        var orderDetail = new List<OrderDetail> { new OrderDetail { Id = orderId } };
        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), true))
            .Returns(orderHeader);
        _unitOfWorkMock.Setup(u => u.OrderDetail.GetAll(It.IsAny<Expression<Func<OrderDetail, bool>>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(orderDetail);

        var controller = new OrderController(_unitOfWorkMock.Object, null!);
        // Act
        var result = controller.Details(orderId);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult!.Model.Should().BeOfType<OrderViewModel>();
        var model = viewResult.Model as OrderViewModel;

        model!.OrderHeader.Should().Be(orderHeader);
        model.OrderDetail.Should().BeEquivalentTo(orderDetail);
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
        _unitOfWorkMock.Setup(u => u.OrderHeader.UpdateStatus(orderHeaderId, It.IsAny<string>(), SD.PaymentStatusApproved));
        _unitOfWorkMock.Setup(u => u.Save());

        var stripeServices = new Mock<StripeServiceProvider>(MockBehavior.Loose);
        stripeServices.Setup(u => u.GetStripeSession(orderHeader.SessionId!)).Returns(session);
        
        var controller = new OrderController(_unitOfWorkMock.Object, stripeServices.Object);

        //Act
        var result = controller.PaymentConfirmation(orderHeaderId);

        //Assert
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStripePaymentID(orderHeaderId, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStatus(orderHeaderId, It.IsAny<string>(), SD.PaymentStatusApproved), Times.Once()); 
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());

        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult!.Model.Should().BeOfType<int>();
        var model = viewResult.Model;

        model.Should().Be(orderHeaderId);
    }

    [Theory]
    [InlineData(1)]
    public void UpdateOrderDetail_WithValidOrderViewModel_UpdatesOrderDetailInUnitOfWorkAndRedriectsToDetails(int orderHeaderId)
    {
        //arrange
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderViewModel.OrderHeader = orderHeader;

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), false))
            .Returns(orderHeader);

        var controller = new OrderController(_unitOfWorkMock.Object, null!);
        controller.TempData = _tempData;

        // Act
        var result = controller.UpdateOrderDetail(orderViewModel);

        //Assert
        _unitOfWorkMock.Verify(u => u.OrderHeader.Update(orderHeader), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;

        tempDataValue.Should().Be("Order Details Updated Successfully");
        redirectResult!.ActionName.Should().Be("Details");
        redirectResult.RouteValues!.Keys.First().Should().Be("orderId");
        redirectResult.RouteValues.Values.First().Should().Be(1);
    }

    [Fact]
    public void StartProcessing_UpdatesOrderHeaderStatus()
    {
        //Arrange
        var orderHeaderId = 1;
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderViewModel.OrderHeader = orderHeader;

        var controller = new OrderController(_unitOfWorkMock.Object, null!);
        controller.TempData = _tempData;
        _unitOfWorkMock.Setup(u => u.OrderHeader.UpdateStatus(orderHeaderId, SD.StatusInProcess, It.IsAny<string>()));

        // Act
        var result = controller.StartProcessing(orderViewModel);

        //Assert
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStatus(orderHeaderId, SD.StatusInProcess, It.IsAny<string>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());

        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;

        tempDataValue.Should().Be("Order processing started");
        redirectResult!.ActionName.Should().Be("Details");
        redirectResult.RouteValues!.Keys.First().Should().Be("orderId");
        redirectResult.RouteValues.Values.First().Should().Be(1);
    }

    [Fact]
    public void ShipOrder_UpdatesOrderHeaderAndRediretcsToDetails()
    {
        var orderHeaderId = 1;
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderViewModel.OrderHeader = orderHeader;

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), false))
            .Returns(orderHeader);

        var controller = new OrderController(_unitOfWorkMock.Object, null!);
        controller.TempData = _tempData;

        //Act
        var result = controller.ShipOrder(orderViewModel);

        //Assert
        _unitOfWorkMock.Verify(u => u.OrderHeader.Update(orderHeader), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());

        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;

        tempDataValue.Should().Be("Order Shipped Successfully");
        redirectResult!.ActionName.Should().Be("Details");
        redirectResult.RouteValues!.Keys.First().Should().Be("orderId");
        redirectResult.RouteValues.Values.First().Should().Be(1);
    }

    [Fact]
    public void CancelOrder_WithPaymentStatusApproved_RefundsPayment()
    {
        //Arrange
        var orderHeaderId = 1;
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderHeader.PaymentStatus = SD.PaymentStatusApproved;
        orderHeader.PaymentIntendId = "test-intent-id";
        orderViewModel.OrderHeader = orderHeader;

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), false))
            .Returns(orderHeader);

        var stripeServices = new Mock<StripeServiceProvider>(MockBehavior.Loose);
        stripeServices.Setup(u => u.GetRefundService(orderHeader.PaymentIntendId));

        var controller = new OrderController(_unitOfWorkMock.Object, stripeServices.Object);
        controller.TempData = _tempData;

        //Act
        var result = controller.CancelOrder(orderViewModel);

        //Assert
        stripeServices.Verify(s => s.GetRefundService(orderHeader.PaymentIntendId), Times.Once());
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStatus(orderHeaderId, SD.StatusCancelled, It.IsAny<string>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());

        //TODO
        //Dokończyć asercje!
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
            Carrier = "FedEx",
            TrackingNumber = "987654321",
            SessionId = "Test-session"
    };
        return orderHeader;
    }
}