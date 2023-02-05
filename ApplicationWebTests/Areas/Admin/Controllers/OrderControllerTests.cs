namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class OrderControllerTests
{
    public void Index_ReturnsViewResult()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var controller = new CategoryController(mockUnitOfWork.Object);

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
    }
}