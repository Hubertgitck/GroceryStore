using Application.Utility;
using Application.Utility.Exceptions;
using ApplicationWeb.Mediator.Commands.PackagingTypeCommands;
using ApplicationWeb.Mediator.Handlers.PackagingTypeHandlers;
using ApplicationWeb.Mediator.Requests.PackagingTypeRequests;
using ApplicationWebUnitTests.Utility;
using AutoMapper;

namespace ApplicationWeb.Mediator.Handlers.CategoryHandlers.Tests;

public class PackagingTypeHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;

    public PackagingTypeHandlerTests()
    {
        _unitOfWorkMock = new ();
        _mapper = AutoMapperInstance.GetAutoMapper();
    }

    [Fact]
    public void AddPackagingTypeHandler_WhenPackagingTypeIsInGrams_ShouldDivideWeightByFactor()
    {
        //Arrange  
        var testPackagingTypeDto = GetTestPackagingTypeDto();
        var weightBeforeHandlerCall = testPackagingTypeDto.Weight;
        _unitOfWorkMock.Setup(u => u.PackagingType.Add(It.IsAny<PackagingType>()));
        var request = new AddPackagingType(testPackagingTypeDto);
        var handler = new AddPackagingTypeHandler(_unitOfWorkMock.Object, _mapper);

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
        var handler = new AddPackagingTypeHandler(_unitOfWorkMock.Object, _mapper);

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
        var handler = new AddPackagingTypeHandler(_unitOfWorkMock.Object, _mapper);

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
        PackagingType packagingTypeFromDb = null!;
        var request = new DeletePackagingTypeById(It.IsAny<int>());
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(u => u.Id == It.IsAny<int>(), default, true))
            .Returns(packagingTypeFromDb);
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
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(u => u.Id == request.Id, default, true)).Returns(packagingType); 
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
        var handler = new EditPackagingTypeHandler(_unitOfWorkMock.Object, _mapper);
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
        var handler = new EditPackagingTypeHandler(_unitOfWorkMock.Object, _mapper);
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
        var handler = new EditPackagingTypeHandler(_unitOfWorkMock.Object, _mapper);
        _unitOfWorkMock.Setup(u => u.PackagingType.Update(It.IsAny<PackagingType>()));

        //Act
        _ = handler.Handle(request, default);

        //Assert
        _unitOfWorkMock.Verify(u => u.PackagingType.Update(It.IsAny<PackagingType>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.Save(), Times.Once());
    }

    [Fact]
    public void GetAllPackagingTypesHandler_WhenPackagingTypeWeightIsInGrams_ShouldMultiplyByFactor()
    {
        //Assert
        var testPackagingTypesList = new List<PackagingType>
        {
            GetTestPackagingType(),
            GetTestPackagingType()
        };
        _unitOfWorkMock.Setup(u => u.PackagingType.GetAll(default,default,default)).Returns(testPackagingTypesList);
        var beforeHandlerCallWeight = testPackagingTypesList.First().Weight;
        var request = new GetAllPackagingTypes();
        var handler = new GetAllPackagingTypesHandler(_unitOfWorkMock.Object, _mapper);

        //Act
        _ = handler.Handle(request, default);

        //Assert
        testPackagingTypesList[0].Weight.Should().Be(beforeHandlerCallWeight * Constants.KilogramsToGramsFactor);
        testPackagingTypesList[1].Weight.Should().Be(beforeHandlerCallWeight * Constants.KilogramsToGramsFactor);
    }    
    
    [Fact]
    public void GetAllPackagingTypesHandler_ShouldReturnPackagingTypesListFromDb()
    {
        //Assert
        var testPackagingTypesList = new List<PackagingType>
        {
            GetTestPackagingType(),
            GetTestPackagingType()
        };
        _unitOfWorkMock.Setup(u => u.PackagingType.GetAll(default,default,default)).Returns(testPackagingTypesList);
        var beforeHandlerCallWeight = testPackagingTypesList.First().Weight;
        var request = new GetAllPackagingTypes();
        var handler = new GetAllPackagingTypesHandler(_unitOfWorkMock.Object, _mapper);

        //Act
        var result = handler.Handle(request, default).Result.ToList();

        //Assert
        result.Should().BeEquivalentTo(testPackagingTypesList);
    }

    [Theory]
    [InlineData(0)]
    public void GetPackagingTypeByIdHandler_WhenIdIsZero_ShouldThrowArgumentException(int packagingTypeId)
    {
        //Assert
        var request = new GetPackagingTypeById(packagingTypeId);
        var handler = new GetPackagingTypeByIdHandler(_unitOfWorkMock.Object, _mapper);

        //Act
        Action act = () => handler.Handle(request, default);

        //Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid id");
    }

    [Theory]
    [InlineData(1)]
    public void GetPackagingTypeByIdHandler_WhenPackagingTypeIsNotFoundInDatabase_ShouldThrowNotFoundException(int packagingTypeId)
    {
        //Assert
        PackagingType packagingTypeFromDb = null!;
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(p => p.Id == packagingTypeId, default, true))
            .Returns(packagingTypeFromDb);
        var request = new GetPackagingTypeById(packagingTypeId);
        var handler = new GetPackagingTypeByIdHandler(_unitOfWorkMock.Object, _mapper);

        //Act
        Action act = () => handler.Handle(request, default);

        //Assert
        act.Should().Throw<NotFoundException>()
            .WithMessage($"PackagingType with ID: {request.Id} was not found in database");
    }   
    [Theory]
    [InlineData(1)]
    public void GetPackagingTypeByIdHandler_WhenPackagingTypeWeightIsInGrams_ShouldDivideWeightByFactor(int packagingTypeId)
    {
        //Assert
        var packagingTypeFromDb = GetTestPackagingType();
        var beforeHandlerCallWeight = packagingTypeFromDb.Weight;
        var request = new GetPackagingTypeById(packagingTypeId);
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(p => p.Id == request.Id, default, true))
            .Returns(packagingTypeFromDb);
        var handler = new GetPackagingTypeByIdHandler(_unitOfWorkMock.Object, _mapper);

        //Act
        _ = handler.Handle(request, default);

        //Assert
        packagingTypeFromDb.Weight.Should().Be(beforeHandlerCallWeight * Constants.KilogramsToGramsFactor);
    }

    [Theory]
    [InlineData(1)]
    public void GetPackagingTypeByIdHandler_WhenEverythingIsOk_ShouldReturnMappedPackagingType(int packagingTypeId)
    {
        //Assert
        var packagingTypeFromDb = GetTestPackagingType();
        var beforeHandlerCallWeight = packagingTypeFromDb.Weight;
        var request = new GetPackagingTypeById(packagingTypeId);
        _unitOfWorkMock.Setup(u => u.PackagingType.GetFirstOrDefault(p => p.Id == request.Id, default, true))
            .Returns(packagingTypeFromDb);
        var handler = new GetPackagingTypeByIdHandler(_unitOfWorkMock.Object, _mapper);

        //Act
        var result = handler.Handle(request, default).Result;

        //Assert
        var expectedResult = _mapper.Map<PackagingTypeDto>(packagingTypeFromDb);

        result.Should().BeEquivalentTo(expectedResult);
    }

    private PackagingTypeDto GetTestPackagingTypeDto()
    {
        return new PackagingTypeDto { Id = 1, IsWeightInGrams= true, Name = "test", Weight = 100};
    }

    private PackagingType GetTestPackagingType()
    {
        return new PackagingType { Id = 1, IsWeightInGrams = true, Name = "test", Weight = 100 };
    }
}