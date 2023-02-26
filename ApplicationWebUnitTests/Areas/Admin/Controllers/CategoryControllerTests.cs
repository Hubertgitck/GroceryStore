using ApplicationWeb.Mediator.Commands.CategoryCommands;
using ApplicationWeb.Mediator.DTO;
using ApplicationWeb.Mediator.Requests.CategoryRequests;
using ApplicationWebTests.TestUtilities;
using MediatR;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class CategoryControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CategoryController(_mediatorMock.Object)
        {
            TempData = TempDataProvider.GetTempDataMock()
        };
    }

    [Fact]
    public async Task Index_ShouldReturnViewResultOfAllCategories()
    {
        //Arrange
        var categoryList = GetTestCategoriesList();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllCategories>(), default)).ReturnsAsync(categoryList);

        //Act
        var result = await _controller.Index();

        //Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult!.Model.Should().BeAssignableTo<IEnumerable<CategoryDto>>();
        viewResult.Model.Should().BeEquivalentTo(categoryList);
    }

    [Fact]
    public void Create_ReturnsViewResult()
    {
        //Act
        var result = _controller.Create();

        //Assert
        var viewResult = result as ViewResult;
        viewResult.Should().BeOfType(typeof(ViewResult));
    }

    [Fact]
    public async Task Create_WithValidModel_ShouldSendAddNewCategoryRequestToMediator()
    {
        //Act
        var result = await _controller.Create(It.IsAny<CategoryDto>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<AddCategory>(), default), Times.Once);
    }

    [Fact]
    public async Task Create_WithInvalidModel_ShouldReturnViewWithThatModel()
    {
        //Arrange
        var category = GetTestCategory();
        _controller.ModelState.AddModelError("Name", "The Name field is required.");
        
        //Act
        var result = await _controller.Create(category);

        //Assert
        var viewResult = result as ViewResult;

        viewResult.Should().BeOfType(typeof(ViewResult));
        viewResult!.Model.Should().BeEquivalentTo(category);
    }

    [Fact]
    public async Task Edit_ShouldSendGetCategoryByGivenIdRequestToMediator()
    {
        //Act
        await _controller.Edit(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(GetCategoryByIdRequest(), default), Times.Once);
    }

    [Fact]
    public async Task Edit_WithCategoryFoundInDatabase_ShouldReturnViewWithThatCategory()
    {
        //Arrange
        var category = GetTestCategory();
        _mediatorMock.Setup(m => m.Send(GetCategoryByIdRequest(), default)).ReturnsAsync(category);

        //Act
        var result = await _controller.Edit(It.IsAny<int>());

        // Assert
        var viewResult = result as ViewResult;

        viewResult.Should().BeOfType(typeof(ViewResult));
        viewResult!.Model.Should().BeEquivalentTo(category);
    }

    [Fact]
    public async Task Edit_WithValidModel_ShouldSendEditCategoryRequestToMediator()
    {
        //Act
        var result = await _controller.Edit(It.IsAny<CategoryDto>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<EditCategory>(), default), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldSendRequestToMediatorToGetCategoryByGivenId()
    {
        //Act
        await _controller.Delete(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(GetCategoryByIdRequest(), default), Times.Once);
    }

    [Fact]
    public async Task DeletePost_ShouldSendDeleteCategoryByIdRequestToMediator()
    {
        //Act
        await _controller.DeletePost(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteCategoryById>(), default), Times.Once);
    }
    
    private CategoryDto GetTestCategory()
    {
        return new CategoryDto { Id = 1, Name = "Category 1" };
    }
    private List<CategoryDto> GetTestCategoriesList()
    {
        var expectedCategories = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Category 1" },
            new CategoryDto { Id = 2, Name = "Category 2" }
        };

        return expectedCategories;
    }
    private GetCategoryById GetCategoryByIdRequest()
    {
        return It.IsAny<GetCategoryById>();
    }
}