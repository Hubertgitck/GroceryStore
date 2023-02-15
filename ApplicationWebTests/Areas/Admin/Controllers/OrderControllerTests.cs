using System.Security.Claims;
using Application.Models.ViewModels;
using Application.Utility;
using ApplicationWeb.Areas.Admin.Controllers;
using ApplicationWebTests.TestUtilities;
using Stripe.Checkout;

namespace ApplicationWebTests.Areas.Admin.Controllers;

public class OrderControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ITempDataDictionary _tempData;
    private readonly Mock<StripeServiceProvider> _stripeServiceMock = new(MockBehavior.Loose);

    public OrderControllerTests()
    {
        _tempData = TempDataProvider.GetTempDataMock();
    }

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Arrange
        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);

        // Act
        var result = controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Theory]
    [InlineData(1)]
    public void Details_ReturnsViewResult_WithCorrectOrderViewModel(int orderHeaderId)
    {
        // Arrange
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        var orderDetail = new List<OrderDetail> { new OrderDetail { Id = orderHeaderId } };
        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), true))
            .Returns(orderHeader);
        _unitOfWorkMock.Setup(u => u.OrderDetail.GetAll(It.IsAny<Expression<Func<OrderDetail, bool>>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(orderDetail);

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);

        // Act
        var result = controller.Details(orderHeaderId);

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

        var stripeServices = new Mock<StripeServiceProvider>();
        _stripeServiceMock.Setup(u => u.GetStripeSession(orderHeader.SessionId!)).Returns(session);

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);

        //Act
        var result = controller.PaymentConfirmation(orderHeaderId);

        //Assert
        _stripeServiceMock.Verify(s => s.GetStripeSession(orderHeader.SessionId!), Times.Once());
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

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object)
        {
            TempData = _tempData
        };

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

    [Theory]
    [InlineData(1)]
    public void StartProcessing_UpdatesOrderHeaderStatus(int orderHeaderId)
    {
        //Arrange
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderViewModel.OrderHeader = orderHeader;

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object)
        {
            TempData = _tempData
        };
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
        redirectResult.RouteValues.Values.First().Should().Be(orderHeaderId);
    }

    [Theory]
    [InlineData(1)]
    public void ShipOrder_UpdatesOrderHeaderAndRediretcsToDetails(int orderHeaderId)
    {
        //Arrange
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderViewModel.OrderHeader = orderHeader;

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), false))
            .Returns(orderHeader);

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);
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
        redirectResult.RouteValues.Values.First().Should().Be(orderHeaderId);
    }

    [Theory]
    [InlineData(1, SD.PaymentStatusApproved)]
    public void CancelOrder_WithPaymentStatusApproved_RefundsPaymentAndRedirectsToDetails(int orderHeaderId, string status)
    {
        //Arrange
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderHeader.PaymentStatus = status;
        orderViewModel.OrderHeader = orderHeader;

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), false))
            .Returns(orderHeader);

        _stripeServiceMock.Setup(u => u.GetRefundService(orderHeader.PaymentIntendId!));

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);
        controller.TempData = _tempData;

        //Act
        var result = controller.CancelOrder(orderViewModel);

        //Assert
        _stripeServiceMock.Verify(s => s.GetRefundService(orderHeader.PaymentIntendId!), Times.Once());
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStatus(orderHeaderId, SD.StatusCancelled, It.IsAny<string>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());

        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;

        tempDataValue.Should().Be("Order Cancelled Successfully");
        redirectResult!.ActionName.Should().Be("Details");
        redirectResult.RouteValues!.Keys.First().Should().Be("orderId");
        redirectResult.RouteValues.Values.First().Should().Be(orderHeaderId);
    }

    [Theory]
    [InlineData(1, SD.PaymentStatusPending)]
    [InlineData(2, SD.PaymentStatusRejected)]
    public void CancelOrder_WithPaymentStatusOtherThanApproved_RedirectsToDetails(int orderHeaderId, string status)
    {
        //Arrange
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        var orderViewModel = new Mock<OrderViewModel>().Object;
        orderHeader.PaymentStatus = status;
        orderViewModel.OrderHeader = orderHeader;

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), false))
            .Returns(orderHeader);

        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);
        controller.TempData = _tempData;

        //Act
        var result = controller.CancelOrder(orderViewModel);

        //Assert
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStatus(orderHeaderId, SD.StatusCancelled, It.IsAny<string>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());

        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;

        tempDataValue.Should().Be("Order Cancelled Successfully");
        redirectResult!.ActionName.Should().Be("Details");
        redirectResult.RouteValues!.Keys.First().Should().Be("orderId");
        redirectResult.RouteValues.Values.First().Should().Be(orderHeaderId);
    }

    [Theory]
    [InlineData("inprocess")]
    public void GetAll_ReturnsJsonDataResult_WhenCalled(string status)
    {
        // Arrange
        var orderHeaders = new List<OrderHeader>
        {
            GetTestOrderHeader(1),
            GetTestOrderHeader(2),
            GetTestOrderHeader(3),
        };
        _unitOfWorkMock.Setup(x => x.OrderHeader.GetAll(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(orderHeaders);
        var controller = new OrderController(_unitOfWorkMock.Object, _stripeServiceMock.Object);
        controller.ControllerContext = GetTestControllerContext(SD.Role_Admin);

        // Act
        var result = controller.GetAll(status);

        // Assert
        result.Should().BeOfType<JsonResult>();
        _unitOfWorkMock.Verify(x => x.OrderHeader.GetAll(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
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
            SessionId = "Test-session",
            PaymentIntendId = "Some-payment-IntendId"
        };
        return orderHeader;
    }

    private ControllerContext GetTestControllerContext(string role = null)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, "userId"),
    };
        if (role != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var context = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
        return context;
    }
}