using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Application.Models;
using Application.Models.ViewModels;
using Application.Utility;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class PackagingTypeControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITempDataProvider> _tempDataProviderMock = new();
    private readonly TempDataDictionaryFactory _tempDataDictionaryFactory;
    private readonly ITempDataDictionary _tempData;

    public PackagingTypeControllerTests()
    {
        _tempDataDictionaryFactory = new TempDataDictionaryFactory(_tempDataProviderMock.Object);
        _tempData = _tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
    }

    [Fact]
    public void Index_ShouldReturnViewResultWithListOfAllPackagingTypes()
    {
        // Arrange
        var packagingTypeList = new List<PackagingType>
        {
            GetTestPackagingType(),
            GetTestPackagingType()
        };
        _unitOfWorkMock.Setup(u => u.PackagingType.GetAll(It.IsAny<Expression<Func<PackagingType, bool>>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(packagingTypeList);
        var controller = new PackagingTypeController(_unitOfWorkMock.Object);

        // Act
        var result = controller.Index();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult!.Model.Should().BeOfType<List<PackagingType>>();
    }

    [Fact]
    public void CreateAction_ShouldReturnViewResult()
    {
        // Arrange
        var controller = new PackagingTypeController(_unitOfWorkMock.Object);

        // Act
        var result = controller.Create();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Create_WithValidModel_ShouldRedirectToIndex()
    {
        //Arrange
        var packagingType = GetTestPackagingType();
        _unitOfWorkMock.Setup(u => u.PackagingType.Add(packagingType));

        var controller = new PackagingTypeController(_unitOfWorkMock.Object);
        controller.TempData = _tempData;

        //Act
        var result = controller.Create(packagingType);

        //Assert
        _unitOfWorkMock.Verify(u => u.PackagingType.Add(packagingType), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());

        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;
        tempDataValue.Should().Be("Packaging Type created succesfully");
        redirectResult!.ActionName.Should().Be("Index");
    }

    [Fact]
    public void Create_WithInvalidModel_ShouldRedirectBackToEditView()
    {
        //Arrange
        var packagingType = GetTestPackagingType();

        var controller = new PackagingTypeController(_unitOfWorkMock.Object);
        controller.ModelState.AddModelError("Name", "Required");

        //Act
        var result = controller.Create(packagingType);

        //Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(packagingType);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void Edit_WithNullOrZeroValuePackagingTypeId_ShouldReturnNotFound(int? packagingTypeId)
    {
        //Arrange
        var controller = new PackagingTypeController(_unitOfWorkMock.Object);

        //Act
        var result = controller.Edit(packagingTypeId);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(1)]
    public void Edit_WithValidPackagingTypeIdButPackagingTypeNotInDatabase_ShouldReturnNotFound(int packagingTypeId)
    {
        //Arrange
        PackagingType packagingType = null!;
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(It.IsAny<Expression<Func<PackagingType, bool>>>(), It.IsAny<string>(), true))
            .Returns(packagingType);
        var controller = new PackagingTypeController(_unitOfWorkMock.Object);

        //Act
        var result = controller.Edit(packagingTypeId);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(1)]
    public void Edit_WithValidPackagingTypeIdAndPackagingTypeFoundInDb_ShouldReturnViewWithThatPackagingType(int packagingTypeId)
    {
        //Arrange
        PackagingType packagingType = GetTestPackagingType();
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(It.IsAny<Expression<Func<PackagingType, bool>>>(), It.IsAny<string>(), true))
            .Returns(packagingType);
        var controller = new PackagingTypeController(_unitOfWorkMock.Object);

        //Act
        var result = controller.Edit(packagingTypeId);

        //Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(packagingType);
    }

    [Fact]
    public void EditPost_WithValidModel_ShouldRedirectToIndex()
    {
        //Arrange
        var packagingType = GetTestPackagingType();
        _unitOfWorkMock.Setup(u => u.PackagingType.Update(packagingType));

        var controller = new PackagingTypeController(_unitOfWorkMock.Object);
        controller.TempData = _tempData;

        //Act
        var result = controller.Edit(packagingType);

        //Assert
        _unitOfWorkMock.Verify(u => u.PackagingType.Update(packagingType), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());

        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;
        tempDataValue.Should().Be("Packaging Type updated succesfully");
        redirectResult!.ActionName.Should().Be("Index");
    }

    [Fact]
    public void EditPost_WithInvalidModel_ShouldRedirectBackToEditView()
    {
        //Arrange
        var packagingType = GetTestPackagingType();

        var controller = new PackagingTypeController(_unitOfWorkMock.Object);
        controller.ModelState.AddModelError("Name", "Required");

        //Act
        var result = controller.Edit(packagingType);

        //Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(packagingType);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void Delete_WithNullOrZeroValue_ShouldReturnNotFound(int? packagingTypeId)
    {
        //Arrange
        var controller = new PackagingTypeController(_unitOfWorkMock.Object);

        //Act
        var result = controller.Delete(packagingTypeId);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(1)]
    public void Delete_WithValidPackagingTypeIdButPackagingTypeNotInDatabase_ShouldReturnNotFound(int packagingTypeId)
    {
        //Arrange
        PackagingType packagingType = null!;
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(It.IsAny<Expression<Func<PackagingType, bool>>>(), It.IsAny<string>(), true))
            .Returns(packagingType);
        var controller = new PackagingTypeController(_unitOfWorkMock.Object);

        //Act
        var result = controller.Delete(packagingTypeId);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(1)]
    public void Delete_WithValidPackagingTypeIdAndPackagingTypeFoundInDb_ShouldReturnViewWithThatPackagingType(int packagingTypeId)
    {
        //Arrange
        PackagingType packagingType = GetTestPackagingType();
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(It.IsAny<Expression<Func<PackagingType, bool>>>(), It.IsAny<string>(), true))
            .Returns(packagingType);
        var controller = new PackagingTypeController(_unitOfWorkMock.Object);

        //Act
        var result = controller.Delete(packagingTypeId);

        //Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;
        viewResult!.Model.Should().Be(packagingType);
    }

    [Theory]
    [InlineData(1)]
    public void DeletePost_WithPackagingTypeNotFoundInDatabase_ShouldReturnNotFound(int packagingTypeId)
    {
        //Arrange
        PackagingType packagingType = null!;
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(It.IsAny<Expression<Func<PackagingType, bool>>>(), It.IsAny<string>(), true))
            .Returns(packagingType);

        var controller = new PackagingTypeController(_unitOfWorkMock.Object);

        //Act
        var result = controller.DeletePOST(packagingTypeId);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
    }
    [Theory]
    [InlineData(1)]
    public void DeletePost_WithPackagingTypeFoundInDatabase_ShouldRedirectToIndex(int packagingTypeId)
    {
        //Arrange
        PackagingType packagingType = GetTestPackagingType();
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(It.IsAny<Expression<Func<PackagingType, bool>>>(), It.IsAny<string>(), true))
            .Returns(packagingType);


        var controller = new PackagingTypeController(_unitOfWorkMock.Object);
        controller.TempData = _tempData;

        //Act
        var result = controller.DeletePOST(packagingTypeId);

        //Assert
        _unitOfWorkMock.Verify(u => u.PackagingType.Remove(packagingType), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());

        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;
        tempDataValue.Should().Be("Packaging Type deleted succesfully");
        redirectResult!.ActionName.Should().Be("Index");
    }

    private PackagingType GetTestPackagingType()
    {
        return new PackagingType() {
            Id = 1,
            Name = "test",
            Weight = 10,
            IsWeightInGrams = true
        };
    }
}