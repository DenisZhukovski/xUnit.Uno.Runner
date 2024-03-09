using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using XUnit.Runners.Core;

namespace Xunit.Uno.Runner;

public class TestCycleResultViewModel : ObservableObject
{
    private readonly ObservableCollection<TestCaseViewModel> _allTests;
    TestState _result = TestState.NotRun;
    TestState _resultFilter;
    RunStatus _runStatus = RunStatus.NotRun;
    int _notRun;
    int _passed;
    int _failed;
    int _skipped;
    string? _detailText;

    public TestCycleResultViewModel(ObservableCollection<TestCaseViewModel> allTests)
    {
        _allTests = allTests;
    }
    
    public TestState Result
    {
        get => _result;
        set => SetProperty(ref _result, value);
    }
    
    public TestState ResultFilter
    {
        get => _resultFilter;
        set
        {
            if (SetProperty(ref _resultFilter, value))
            {
                // FilterAfterDelay();
            }
        }
    }
    
    public RunStatus RunStatus
    {
        get => _runStatus;
        private set => SetProperty(ref _runStatus, value);
    }
    
    public int NotRun
    {
        get => _notRun;
        set => SetProperty(ref _notRun, value);
    }
    
    public int Passed
    {
        get => _passed;
        set => SetProperty(ref _passed, value);
    }
    
    public int Failed
    {
        get => _failed;
        set => SetProperty(ref _failed, value);
    }
    
    public int Skipped
    {
        get => _skipped;
        set => SetProperty(ref _skipped, value);
    }
    
    public string DetailText
    {
        get => _detailText ?? string.Empty;
        private set => SetProperty(ref _detailText, value);
    }
    
    public void UpdateCaption()
    {
    	var count = _allTests.Count;
    	if (count == 0)
    	{
    		DetailText = "No tests were found inside this assembly";
    		RunStatus = RunStatus.NoTests;
    		return;
    	}
    
    	var results = _allTests
    		.GroupBy(r => r.TestResult.State)
    		.ToDictionary(k => k.Key, v => v.Count());
    
    	results.TryGetValue(TestState.Passed, out int passed);
    	results.TryGetValue(TestState.Failed, out int failure);
    	results.TryGetValue(TestState.Skipped, out int skipped);
    	results.TryGetValue(TestState.NotRun, out int notRun);
    
    	Passed = passed;
    	Failed = failure;
    	Skipped = skipped;
    	NotRun = notRun;
    
    	var prefix = notRun == 0 ? "Complete - " : string.Empty;
    
    	if (failure == 0 && notRun == 0)
    	{
    		// No failures and all run
    
    		DetailText = $"{prefix}✔ {passed}";
    		RunStatus = RunStatus.Ok;
    
    		Result = TestState.Passed;
    	}
    	else if (failure > 0 || (notRun > 0 && notRun < count))
    	{
    		// Either some failed or some are not run
    
    		DetailText = $"{prefix}✔ {passed}, ⛔ {failure}, ⚠ {skipped}, 🔷 {notRun}";
    
    		if (failure > 0) // always show a fail
    		{
    			RunStatus = RunStatus.Failed;
    			Result = TestState.Failed;
    		}
    		else
    		{
    			if (passed > 0)
    			{
    				RunStatus = RunStatus.Ok;
    				Result = TestState.Passed;
    			}
    			else if (skipped > 0)
    			{
    				RunStatus = RunStatus.Skipped;
    				Result = TestState.Skipped;
    			}
    			else
    			{
    				// just not run
    				RunStatus = RunStatus.NotRun;
    				Result = TestState.NotRun;
    			}
    		}
    	}
    	else if (Result == TestState.NotRun)
    	{
    		// Not run
    
    		DetailText = $"🔷 {count}, {Result}";
    		RunStatus = RunStatus.NotRun;
    	}
    }
}
