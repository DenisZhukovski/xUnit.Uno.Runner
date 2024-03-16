using XUnit.Runners.Core;

namespace Xunit.Uno.Runner;

public class TestsFilterViewModel
{
    private readonly IFilteredCollection<TestCaseViewModel> _filteredTests;
    private readonly ITestFilter _testFilter = new TestFilter();

    public TestsFilterViewModel(IFilteredCollection<TestCaseViewModel> filteredTests)
    {
        _filteredTests = filteredTests;
        _filteredTests.Filter = _testFilter;
    }
    
    public IList<TestState> States => Enum.GetValues<TestState>();
    
    public string SearchQuery
    {
        get => _testFilter.Name;
        set
        {
            if (_testFilter.Name !=  value)
            {
                _testFilter.Name = value;
                _filteredTests.Filter = _testFilter;
            }
        }
    }
        
    public TestState StateFilter
    {
        get => _testFilter.State;
        set
        {
            if (_testFilter.State !=  value)
            {
                _testFilter.State = value;
                _filteredTests.Filter = _testFilter;
            }
        }
    }
}
