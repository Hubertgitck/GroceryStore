using Xunit;
using Application.DataAccess.Repositories.IRepository;
using Moq;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class CategoryControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ITempDataProvider _tempDataProvider;
    private readonly TempDataDictionaryFactory _tempDataDictionaryFactory;
    private readonly ITempDataDictionary _tempData;
    public CategoryControllerTests()
    {
        _tempDataProvider = Mock.Of<ITempDataProvider>();
        _tempDataDictionaryFactory = new TempDataDictionaryFactory(_tempDataProvider);
        _tempData = _tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
    }

    [Fact]
    public void Index_ReturnsAViewResultWithAListOfCategories()
    {
        // Arrange
        var categoryList = GetCategoryTestList();
        _unitOfWorkMock.Setup(u => u.Category.GetAll(default,default,default)).Returns(categoryList);
        var controller = new CategoryController(_unitOfWorkMock.Object);

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Category>>(viewResult.Model);
        Assert.Equal(categoryList, model);
    }

    [Fact]
    public void Create_ReturnsAViewResult()
    {
        // Arrange
        var controller = new CategoryController(_unitOfWorkMock.Object);

        // Act
        var result = controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Create_WithValidCategory_AddsCategoryToUnitOfWork()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Category.Add(It.IsAny<Category>()));
        _unitOfWorkMock.Setup(u => u.Save());

        var controller = new CategoryController(_unitOfWorkMock.Object);
        controller.TempData = _tempData;
        var category = GetTestCategory();

        // Act
        var result = controller.Create(category);

        // Assert
        _unitOfWorkMock.Verify(u => u.Category.Add(category), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Fact]
    public void Create_WithValidCategory_RedirectsToIndex()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.Category.Add(It.IsAny<Category>()));
        _unitOfWorkMock.Setup(u => u.Save());
        var controller = new CategoryController(_unitOfWorkMock.Object);
        controller.TempData = _tempData;
        var category = GetTestCategory();

        // Act
        var result = controller.Create(category);

        // Assert
        var tempDataValue = Assert.IsType<string>(controller.TempData["success"]);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Category created succesfully", tempDataValue);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public void Create_WithInvalidCategory_ReturnsViewResultWithSameCategory()
    {
        // Arrange
        var controller = new CategoryController(_unitOfWorkMock.Object);
        controller.ModelState.AddModelError("Name", "Required");
        var category = new Category();
        controller.TempData = _tempData;

        // Act
        var result = controller.Create(category);

        // Assert
        _unitOfWorkMock.Verify(u => u.Category.Add(It.IsAny<Category>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Never());

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(category, viewResult.Model);
    }

    private IEnumerable<Category> GetCategoryTestList()
    {
        var categoryList = new List<Category>
        {
            new Category { Id = 1, Name = "Category 1" },
            new Category { Id = 2, Name = "Category 2" },
            new Category { Id = 3, Name = "Category 3" }
        };
        return categoryList;
    }
    public static Category GetTestCategory()
    {
        return new Category { Id = 1, Name = "Test", DisplayOrder = 1 };
    }
}