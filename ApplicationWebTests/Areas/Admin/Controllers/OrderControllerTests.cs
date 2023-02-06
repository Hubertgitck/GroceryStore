using System.Linq.Expressions;
using Application.Models;
using Application.Models.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class OrderControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Arrange
        var controller = new OrderController(_unitOfWorkMock.Object);

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        viewResult.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Details_ReturnsViewResult_WithCorrectOrderViewModel()
    {
        // Arrange
        var orderId = 1;
        var orderHeader = new OrderHeader();
        var orderDetail = new List<OrderDetail>();
        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>(), true))
            .Returns(orderHeader);
        _unitOfWorkMock.Setup(u => u.OrderDetail.GetAll(It.IsAny<Expression<Func<OrderDetail, bool>>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(orderDetail);

        var controller = new OrderController(_unitOfWorkMock.Object);
        // Act
        var result = controller.Details(orderId);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult.Model.Should().BeOfType<OrderViewModel>();
        var model = viewResult.Model as OrderViewModel;

        model.OrderHeader.Should().Be(orderHeader);
        model.OrderDetail.Should().BeEquivalentTo(orderDetail);
    }
}