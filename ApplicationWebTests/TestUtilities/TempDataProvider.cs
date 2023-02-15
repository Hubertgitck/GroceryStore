using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ApplicationWebTests.TestUtilities;

public static class TempDataProvider
{
    static readonly ITempDataProvider _tempDataProvider;
    static readonly TempDataDictionaryFactory _tempDataDictionaryFactory;
    static readonly ITempDataDictionary _tempData;

    static TempDataProvider()
    {
        _tempDataProvider = Mock.Of<ITempDataProvider>();
        _tempDataDictionaryFactory = new TempDataDictionaryFactory(_tempDataProvider);
        _tempData = _tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
    }

    public static ITempDataDictionary GetTempDataMock()
    {
        return _tempData;
    }
}

