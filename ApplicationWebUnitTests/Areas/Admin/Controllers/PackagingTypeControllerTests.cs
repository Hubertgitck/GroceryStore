namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class PackagingTypeControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public PackagingTypeControllerTests()
    {
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