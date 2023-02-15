using Application.Utility;

namespace ApplicationWebTests.Areas.Admin.Controllers;

public class OrderControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<StripeServiceProvider> _stripeServiceMock = new(MockBehavior.Loose);

    public OrderControllerTests()
    {
    }

}