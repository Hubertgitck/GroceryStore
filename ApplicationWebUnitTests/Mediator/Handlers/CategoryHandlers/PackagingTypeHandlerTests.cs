using Application.Utility;
using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Commands.PackagingTypeCommands;
using ApplicationWeb.Mediator.Handlers.PackagingTypeHandlers;
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
    public void AddPackagingTypeHandler_ShouldCallAddPackagingTypeToDatabase()
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

    [Fact]
    public void DeletePackagingTypeByIdHandler_WhenPackagingTypeIsNotFoundInDatabase_ShouldThrowNotFoundExpcetion()
    {
        //Arrange  
        PackagingType packagingType = null!;
        var request = new DeletePackagingTypeById(It.IsAny<int>());
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(u => u.Id == It.IsAny<int>(), null, true)).Returns(packagingType);
        var handler = new DeletePackagingTypeByIdHandler(_unitOfWorkMock.Object);

        //Act
        Action act = () => handler.Handle(request, default);

        //Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage($"Order Header with ID: {request.Id} was not found in database");
    }    
    
    [Fact]
    public void DeletePackagingTypeByIdHandler_WhenPackagingTypeIsFoundInDatabase_ShouldRemoveItFromDatabase()
    {
        //Arrange  
        PackagingType packagingType = new Mock<PackagingType>().Object;
        var request = new DeletePackagingTypeById(It.IsAny<int>());
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(u => u.Id == request.Id, null, true)).Returns(packagingType); 
        var handler = new DeletePackagingTypeByIdHandler(_unitOfWorkMock.Object);

        //Act
        _ = handler.Handle(request, default);

        //Assert
        _unitOfWorkMock.Verify(u => u.PackagingType.Remove(packagingType), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Fact]
    public void EditPackagingTypeHandler_WhenPackagingTypeIsInGrams_ShouldDivideWeightByFactor()
    {
        //Arrange  
        var testPackagingTypeDto = GetTestPackagingTypeDto();
        var beforeHandlerCallWeight = testPackagingTypeDto.Weight;
        var request = new EditPackagingType(testPackagingTypeDto);
        var handler = new EditPackagingTypeHandler(_unitOfWorkMock.Object, _mapperMock);
        _unitOfWorkMock.Setup(u => u.PackagingType.Update(It.IsAny<PackagingType>()));

        //Act
        _ = handler.Handle(request, default);

        //Assert
        request.PackagingTypeDto.Weight.Should().Be(beforeHandlerCallWeight /= Constants.KilogramsToGramsFactor);
    }

    [Fact]
    public void EditPackagingTypeHandler_WhenPackagingTypeIsNotInGrams_ShouldNotDivideWeight()
    {
        //Arrange  
        var testPackagingTypeDto = GetTestPackagingTypeDto();
        testPackagingTypeDto.IsWeightInGrams = false;
        var beforeHandlerCallWeight = testPackagingTypeDto.Weight;
        var request = new EditPackagingType(testPackagingTypeDto);
        var handler = new EditPackagingTypeHandler(_unitOfWorkMock.Object, _mapperMock);
        _unitOfWorkMock.Setup(u => u.PackagingType.Update(It.IsAny<PackagingType>()));


        //Act
        _ = handler.Handle(request, default);

        //Assert
        request.PackagingTypeDto.Weight.Should().Be(beforeHandlerCallWeight);
    }

    [Fact]
    public void EditPackagingTypeHandler_ShouldCallAddPackagingTypeToDatabase()
    {
        //Arrange  
        PackagingTypeDto packagingType = new Mock<PackagingTypeDto>().Object;
        var request = new EditPackagingType(packagingType);
        var handler = new EditPackagingTypeHandler(_unitOfWorkMock.Object, _mapperMock);
        _unitOfWorkMock.Setup(u => u.PackagingType.Update(It.IsAny<PackagingType>()));

        //Act
        _ = handler.Handle(request, default);

        //Assert
        _unitOfWorkMock.Verify(u => u.PackagingType.Update(It.IsAny<PackagingType>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    private PackagingTypeDto GetTestPackagingTypeDto()
    {
        return new PackagingTypeDto { Id = 1, IsWeightInGrams= true, Name = "test", Weight = 100};
    }
}