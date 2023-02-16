/*using Application.Models;
using ApplicationWebTests.TestUtilities;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class CategoryControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ITempDataDictionary _tempData;

    public CategoryControllerTests()
    {
        _tempData = TempDataProvider.GetTempDataMock();
    }

    [Fact]
    public void Create_WithValidModel_AddsCategoryToUnitOfWork()
    {
        // Arrange
        var category = GetTestCategory();
        _unitOfWorkMock.Setup(u => u.Category.Add(category));
        _unitOfWorkMock.Setup(u => u.Save());
        var controller = new CategoryController(_unitOfWorkMock.Object);
        controller.TempData = _tempData;

        // Act
        var result = controller.Create(category);

        // Assert
        _unitOfWorkMock.Verify(u => u.Category.Add(category), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void Edit_WithNullOrZeroId_ReturnsNotFound(int? categoryId)
    {
        //Arrange
        var controller = new CategoryController(_unitOfWorkMock.Object);

        //Act
        var result = controller.Edit(categoryId);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(1)]
    public void Edit_WithNonExistingCategory_ReturnsNotFound(int categoryId)
    {
        //Arrange
        Category category = null!;
        _unitOfWorkMock.Setup(u => u.Category.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string>(), true))
            .Returns(category);
        var controller = new CategoryController(_unitOfWorkMock.Object);

        //Act
        var result = controller.Edit(categoryId);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void EditPost_WithValidModel_UpdatesCategoryInUnitOfWork()
    {
        //Arrange
        var category = GetTestCategory();
        _unitOfWorkMock.Setup(u => u.Category.Update(category));
        var controller = new CategoryController(_unitOfWorkMock.Object);
        controller.TempData = _tempData;

        //Act
        var result = controller.Edit(category);

        //Assert
        _unitOfWorkMock.Verify(u => u.Category.Update(category), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void Delete_WithNullOrZeroId_ReturnsNotFound(int? categoryId)
    {
        //Arrange
        var controller = new CategoryController(_unitOfWorkMock.Object);

        //Act
        var result = controller.Delete(categoryId);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(1)]
    public void Delete_WithNonExistingCategory_ReturnsNotFound(int categoryId)
    {
        //Arrange
        Category category = null!;
        _unitOfWorkMock.Setup(u => u.Category.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string>(), true))
            .Returns(category);
        var controller = new CategoryController(_unitOfWorkMock.Object);

        //Act
        var result = controller.Delete(categoryId);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void DeletePost_WithNonExistingCategory_ReturnsNotFound()
    {
        //Arrange
        Category category = null!;
        _unitOfWorkMock.Setup(u => u.Category.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string>(), true))
            .Returns(category);
        var controller = new CategoryController(_unitOfWorkMock.Object);

        //Act
        var result = controller.DeletePost(It.IsAny<int>());

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    private Category GetTestCategory()
    {
        return new Category { Id = 1, Name = "Test", DisplayOrder = 1 };
    }
}*/