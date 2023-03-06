
using Application.Utility;
using ApplicationWeb.Mediator.Commands.PackagingTypeCommands;
using AutoMapper;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers.Tests;

public class PackagingTypeHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapperMock;

    public PackagingTypeHandlerTests()
    {
        _unitOfWorkMock = new ();
        _mapperMock = new Mock<IMapper>().Object;
    }

    [Fact]
    public void AddPackagingTypeHandler_WhenPackagingTypeIsInGrams_ShouldDivideWeightByFactor()
    {
        //Arrange  
        var testPackagingTypeDto = GetTestPackagingTypeDto();
        var weightBeforeHandlerCall = testPackagingTypeDto.Weight;
        _unitOfWorkMock.Setup(u => u.PackagingType.Add(It.IsAny<PackagingType>()));
        var request = new AddPackagingType(testPackagingTypeDto);
        var handler = new AddPackagingTypeHandler(_unitOfWorkMock.Object, _mapperMock);

        //Act
        _ = handler.Handle(request, default);

        //Assert
        request.PackagingTypeDto.Weight.Should().Be(weightBeforeHandlerCall /= Constants.KilogramsToGramsFactor); 
    }

    [Fact]
    public void AddPackagingTypeHandler_WhenPackagingTypeIsNotInGrams_ShouldNotDivideWeight()
    {
        //Arrange  
        var testPackagingTypeDto = GetTestPackagingTypeDto();
        testPackagingTypeDto.IsWeightInGrams = false;
        var weightBeforeHandlerCall = testPackagingTypeDto.Weight;
        _unitOfWorkMock.Setup(u => u.PackagingType.Add(It.IsAny<PackagingType>()));
        var request = new AddPackagingType(testPackagingTypeDto);
        var handler = new AddPackagingTypeHandler(_unitOfWorkMock.Object, _mapperMock);

        //Act
        _ = handler.Handle(request, default);

        //Assert
        request.PackagingTypeDto.Weight.Should().Be(weightBeforeHandlerCall);
    }

    [Fact]
    public void AddPackagingTypeHandler_ShouldCallAddToRepositoryAndSaveChanges()
    {
        //Arrange  
        var testPackagingTypeDto = GetTestPackagingTypeDto();
        _unitOfWorkMock.Setup(u => u.PackagingType.Add(It.IsAny<PackagingType>()));
        var request = new AddPackagingType(testPackagingTypeDto);
        var handler = new AddPackagingTypeHandler(_unitOfWorkMock.Object, _mapperMock);

        //Act
        _ = handler.Handle(request, default);

        //Assert
        _unitOfWorkMock.Verify(u => u.PackagingType.Add(It.IsAny<PackagingType>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }


    private PackagingTypeDto GetTestPackagingTypeDto()
    {
        return new PackagingTypeDto { Id = 1, IsWeightInGrams= true, Name = "test", Weight = 100};
    }
}