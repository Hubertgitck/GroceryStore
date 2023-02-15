using Moq;

namespace ApplicationWeb.Areas.Admin.Controllers.Tests;

public class CategoryControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ITempDataProvider _tempDataProvider;
    private readonly TempDataDictionaryFactory _tempDataDictionaryFactory;
    private readonly ITempDataDictionary _tempData;
    public CategoryControllerTests()
    {
        _tempDataProvider = Mock.Of<ITempDataProvider>();
        _tempDataDictionaryFactory = new TempDataDictionaryFactory(_tempDataProvider);
        _tempData = _tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
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