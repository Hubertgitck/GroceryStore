using Application.Models;
using Application.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class ProductControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITempDataProvider> _tempDataProviderMock = new();
    private readonly TempDataDictionaryFactory _tempDataDictionaryFactory;
    private readonly ITempDataDictionary _tempData;
    private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock = new();

    public ProductControllerTests()
    {
        _tempDataDictionaryFactory = new TempDataDictionaryFactory(_tempDataProviderMock.Object);
        _tempData = _tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
    }

    [Fact]
    public void Index_ReturnsViewResult()
    {
        //Arrange
        var controller = new ProductController(_unitOfWorkMock.Object, _webHostEnvironmentMock.Object);

        //Act
        var result = controller.Index();

        //Assert
        result.Should().BeOfType<ViewResult>();
    }
    
    [Theory]
    [Ignore]
    [InlineData(null)]
    [InlineData(0)]
    public void Upsert_WhenInsertingNewProduct_ReturnsViewResultWithNewEmptyProductViewModel(int? productId)
    {
        //Arrange
        var productViewModel = new Mock<ProductViewModel>().Object;
        var categoryList = new Mock<List<Category>>().Object;
        var packagingTypeList = new Mock<List<PackagingType>>().Object;
        var selectListItem = new Mock<SelectListItem>().Object;

        _unitOfWorkMock.Setup(u => u.Category.GetAll(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(categoryList);
        _unitOfWorkMock.Setup(u => u.PackagingType.GetAll(It.IsAny<Expression<Func<PackagingType, bool>>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(packagingTypeList);

        var controller = new ProductController(_unitOfWorkMock.Object, _webHostEnvironmentMock.Object);

        //Act
        var result = controller.Upsert(productId, productViewModel);

        //Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result as ViewResult;

        viewResult!.Model.Should().BeOfType<ProductViewModel>();
        var model = viewResult.Model as ProductViewModel;

        model.Should().Be(productViewModel);
    }

    [Theory]
    [InlineData(0)]
    public void Upsert_WithValidModelAndNonExistingProductInDatabase_AddsProductAndRedirectsToIndex(int productId)
    {
        //Arrange
        var productViewModel = new Mock<ProductViewModel>().Object;
        productViewModel.Product = new Mock<Product>().Object;
        productViewModel.Product.Id = productId;

        _unitOfWorkMock.Setup(u => u.Product.Add(productViewModel.Product));

        var controller = new ProductController(_unitOfWorkMock.Object, _webHostEnvironmentMock.Object);
        controller.TempData = _tempData;
        //Act
        var result = controller.Upsert(productViewModel, null);

        //Assert
        _unitOfWorkMock.Verify(u => u.Product.Add(productViewModel.Product), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;

        tempDataValue.Should().Be("Product created successfully");
        redirectResult!.ActionName.Should().Be("Index");
    }

    [Theory]
    [InlineData(1)]
    public void Upsert_WithValidModelAndExistingProductInDatabase_UpdatesProductAndRedirectsToIndex(int productId)
    {
        //Arrange
        var productViewModel = new Mock<ProductViewModel>().Object;
        productViewModel.Product = new Mock<Product>().Object;
        productViewModel.Product.Id = productId;

        _unitOfWorkMock.Setup(u => u.Product.Update(productViewModel.Product));

        var controller = new ProductController(_unitOfWorkMock.Object, _webHostEnvironmentMock.Object);
        controller.TempData = _tempData;
        //Act
        var result = controller.Upsert(productViewModel, null);

        //Assert
        _unitOfWorkMock.Verify(u => u.Product.Update(productViewModel.Product), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
        var tempDataValue = controller.TempData["success"] as string;
        var redirectResult = result as RedirectToActionResult;

        tempDataValue.Should().Be("Product updated successfully");
        redirectResult!.ActionName.Should().Be("Index");
    }
}