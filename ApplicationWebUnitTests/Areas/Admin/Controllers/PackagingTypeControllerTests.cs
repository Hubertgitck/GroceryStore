using ApplicationWeb.Mediator.Commands.PackagingTypeCommands;
using ApplicationWeb.Mediator.Requests.PackagingTypeRequests;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class PackagingTypeControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly PackagingTypeController _controller;

    public PackagingTypeControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new PackagingTypeController(_mediatorMock.Object)
        {
            TempData = TempDataProvider.GetTempDataMock()
        };
    }

    [Fact]
    public async Task Index_ShouldReturnViewResultOfPackagingTypes()
    {
        //Arrange
        var packagingTypesList = GetTestPackagingTypeList();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllPackagingTypes>(), default)).ReturnsAsync(packagingTypesList);

        //Act
        var result = await _controller.Index();

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetAllPackagingTypes>(), default), Times.Once);

        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult!.Model.Should().BeAssignableTo<IEnumerable<PackagingTypeDto>>();
        viewResult.Model.Should().BeEquivalentTo(packagingTypesList);
    }

    [Fact]
    public async Task Create_WithValidModel_ShouldSendCreatePackagingTypeRequest()
    {
        //Act
        var result = await _controller.Create(It.IsAny<PackagingTypeDto>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<AddPackagingType>(), default), Times.Once);
    }

    [Fact]
    public async Task Create_WithInvalidModel_ShouldReturnViewWithThatModel()
    {
        //Arrange
        var packagingType = GetTestPackagingType();
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        //Act
        var result = await _controller.Create(packagingType);

        //Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult!.Model.Should().BeEquivalentTo(packagingType);
    }

    [Fact]
    public async Task Edit_ShouldSendGetPackagingTypeByGivenIdRequest()
    {
        //Act
        var result = await _controller.Edit(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetPackagingTypeById>(), default), Times.Once);
    }

    [Fact]
    public async Task Edit_WithValidModel_ShouldSendEditPackagingTypeRequest()
    {
        //Act
        var result = await _controller.Edit(It.IsAny<PackagingTypeDto>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<EditPackagingType>(), default), Times.Once);
    }

    [Fact]
    public async Task Edit_WithInvalidModel_ShouldReturnViewWithThatModel()
    {
        //Arrange
        var packagingType = GetTestPackagingType();
        _controller.ModelState.AddModelError("Name", "The Name field is required.");

        //Act
        var result = await _controller.Edit(packagingType);

        //Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult!.Model.Should().BeEquivalentTo(packagingType);
    }

    [Fact]
    public async Task Delete_ShouldSendGetPackagingTypeByGivenIdRequest()
    {
        //Act
        var result = await _controller.Delete(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetPackagingTypeById>(), default), Times.Once);
    }

    [Fact]
    public async Task DeletePost_ShouldSendDeletePackagingTypeByGivenIdRequest()
    {
        //Act
        var result = await _controller.Delete(It.IsAny<int>());

        //Assert
        _mediatorMock.Verify(x => x.Send(It.IsAny<GetPackagingTypeById>(), default), Times.Once);
    }

    private PackagingTypeDto GetTestPackagingType()
    {
        return new PackagingTypeDto()
        {
            Id = 1,
            Name = "test",
            Weight = 10,
            IsWeightInGrams = true
        };
    }

    private List<PackagingTypeDto> GetTestPackagingTypeList()
    {
        var testList = new List<PackagingTypeDto> 
        { 
            new PackagingTypeDto{ Id = 1, Name = "testName", Weight = 50, IsWeightInGrams = true },
            new PackagingTypeDto{ Id = 2, Name = "testName2", Weight = 100, IsWeightInGrams = false }
        };
        return testList;
    }
}