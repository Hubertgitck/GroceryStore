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
    public void Index_ReturnsAViewResultWithAListOfCategories()
    {
        // Arrange
        var categoryList = GetCategoryTestList();
        _unitOfWorkMock.Setup(u => u.Category.GetAll(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(categoryList);
        var controller = new CategoryController(_unitOfWorkMock.Object);

        // Act
        var result = controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult!.Model.Should().BeAssignableTo<IEnumerable<Category>>();
        viewResult.Model.Should().BeEquivalentTo(categoryList);
    }

    [Fact]
    public void Create_ReturnsViewResult()
    {
        // Arrange
        var controller = new CategoryController(_unitOfWorkMock.Object);

        // Act
        var result = controller.Create();

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().BeOfType(typeof(ViewResult));
    }

    [Fact]
    public void Create_WithValidCategory_AddsCategoryToUnitOfWorkAndRedirectsToIndex()
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
        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;

        tempDataValue.Should().Be("Category created succesfully");
        redirectResult!.ActionName.Should().Be("Index");
    }

    [Fact]
    public void Create_WithInvalidCategory_ReturnsViewResultWithThatCategory()
    {
        //Arrange
        var category = GetTestCategory();
        var controller = new CategoryController(_unitOfWorkMock.Object);

        controller.ModelState.AddModelError("Name", "Required");
        controller.TempData = _tempData;

        //Act
        var result = controller.Create(category);

        //Assert
        _unitOfWorkMock.Verify(u => u.Category.Add(It.IsAny<Category>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Never());

        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(category);
    }

    [Theory]
    [InlineData(1)]
    public void Edit_WithExistingCategory_ReturnsViewWithThatCategory(int categoryId)
    {
        //Arrange
        var category = GetTestCategory();
        _unitOfWorkMock.Setup(u => u.Category.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string>(), true))
            .Returns(category);

        var controller = new CategoryController(_unitOfWorkMock.Object);

        //Act
        var result = controller.Edit(categoryId);

        //Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult!.Model.Should().Be(category);
    }

    [Fact]
    public void EditPost_WithValidCategory_RedirectsToIndex()
    {
        // Arrange
        var category = GetTestCategory();
        _unitOfWorkMock.Setup(u => u.Category.Update(category));
        _unitOfWorkMock.Setup(u => u.Save());
        var controller = new CategoryController(_unitOfWorkMock.Object);
        controller.TempData = _tempData;

        // Act
        var result = controller.Edit(category);

        // Assert
        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;

        tempDataValue.Should().Be("Category updated succesfully");
        redirectResult!.ActionName.Should().Be("Index");
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
    private Category GetTestCategory()
    {
        return new Category { Id = 1, Name = "Test", DisplayOrder = 1 };
    }
}