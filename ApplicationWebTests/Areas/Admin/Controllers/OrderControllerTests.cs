using System.Linq.Expressions;
using Application.Models.ViewModels;
using Application.Utility;
using Stripe.Checkout;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class OrderControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

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

        var controller = new OrderController(_unitOfWorkMock.Object, null);
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
        string paymentStatus = SD.PaymentStatusApproved;
        var orderHeader = GetTestOrderHeader(orderHeaderId);
        orderHeader.SessionId = "teest-session";

        var session = new Session { PaymentStatus = "paid", PaymentIntentId = "some-payment-intent-id" };


        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), true))
            .Returns(orderHeader);
        _unitOfWorkMock.Setup(u => u.OrderHeader.UpdateStripePaymentID(orderHeaderId, It.IsAny<string>(), It.IsAny<string>()));
        _unitOfWorkMock.Setup(u => u.OrderHeader.UpdateStatus(orderHeaderId, It.IsAny<string>(), paymentStatus));
        _unitOfWorkMock.Setup(u => u.Save());

        var sessionProviderMock = new Mock<StripeSessionProvider>(MockBehavior.Loose);
        sessionProviderMock.Setup(u => u.GetStripeSession(orderHeader.SessionId)).Returns(session);
        
        var controller = new OrderController(_unitOfWorkMock.Object, sessionProviderMock.Object);

        //Act
        var result = controller.PaymentConfirmation(orderHeaderId);

        //Assert
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStripePaymentID(orderHeaderId, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.OrderHeader.UpdateStatus(orderHeaderId, It.IsAny<string>(), paymentStatus), Times.Once()); 
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    private OrderHeader GetTestOrderHeader(int id)
    {
        return new OrderHeader { Id = id };
    }
}