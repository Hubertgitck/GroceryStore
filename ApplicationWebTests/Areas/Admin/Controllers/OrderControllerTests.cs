using Application.Models.ViewModels;
using FluentAssertions;

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

/*    [Theory]
    [InlineData(1)]
    public IActionResult Details(int orderId)
    {
        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault();
        OrderViewModel = new OrderViewModel()
        {
            OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
            OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product", thenIncludeProperty: "PackagingType"),
        };

        return View(OrderViewModel);
    }*/
}