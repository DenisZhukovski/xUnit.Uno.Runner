using System.Collections.ObjectModel;
using Dotnet.Commands;
using XUnit.Runners.Core;
using Xunit.Uno.Runner.Extensions;

namespace Xunit.Uno.Runner
{
	public class TestCasesViewModel : UIBindableBase
	{
		private readonly ObservableCollection<TestCaseViewModel> _allTests = new();
        private readonly IFilteredCollection<TestCaseViewModel> _filteredTests;
        private readonly ITestCases _testCases;
        readonly INavigator _navigation;
        private readonly ICommands _commands;
        private CancellationToken? _progress;
        private readonly ITestFilter _testFilter = new TestFilter();

        internal TestCasesViewModel(
            ITestCases testCases,
            INavigator navigation,
            ICommands commands)
		{
            _testCases = testCases;
            _navigation = navigation;
            _commands = commands.Cached();

            TestCycleResult = new TestCycleResultViewModel(_allTests);
            _filteredTests = new FilteredCollection<TestCaseViewModel>(_allTests).Delayed();
            _filteredTests.List.CollectionChanged += (_, _) => TestCycleResult.UpdateCaption();
            _filteredTests.Filter = _testFilter;
            
            InitCommand.Execute();
		}

        private CancellationToken? Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    RunAllTestsCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(IsBusy));
                }
            }
        }
        
        public bool IsBusy => Progress != null;
        
        public string DisplayName => Path.GetFileNameWithoutExtension(_testCases.GroupName);
        
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
        
        public string StateFilter
        {
            get => _testFilter.State.ToString();
            set
            {
                if (_testFilter.State.ToString() !=  value)
                {
                    _testFilter.State = Enum.Parse<TestState>(value);
                    _filteredTests.Filter = _testFilter;
                }
            }
        }
        
        public TestCycleResultViewModel TestCycleResult { get; }

        public IList<TestCaseViewModel> TestCases => _filteredTests.List;

        public IAsyncCommand InitCommand => _commands.AsyncCommand(async token =>
        {
            try
            {
                Progress = token;
                var tests = await _testCases.ToViewModels(token);
                var orderedTests = tests.OrderBy(test => test.DisplayName);
                await OnUIAsync(() => _allTests.ReplaceWith(orderedTests));
                TestCycleResult.UpdateCaption();
            }
            finally
            {
                Progress = null;
            }
        });
        
        public IAsyncCommand RunAllTestsCommand => _commands.AsyncCommand(async token =>
        {
            ITestCycle? testCycle = null;
            try
            {
                Progress = token;
                TestCycleResult.Clear();
                testCycle = _testCases.TestCycle;
                testCycle.TestFinished += OnTestFinished;
                await testCycle.RunAsync(
                    TestCases.Select(tc => tc.TestCase).ToList(),
                    token
                );
            }
            finally
            {
                Progress = null;
                if (testCycle != null)
                {
                    testCycle.TestFinished -= OnTestFinished;
                }
            }

        }, () => !IsBusy);

        public ICommand NavigateToResultCommand => _commands.AsyncCommand<TestCaseViewModel?>(
            async (testCase, token) =>
        {
            if (testCase != null)
            {
                await testCase.RunAsync(token);
                //await _navigation.NavigateAsync(PageType.TestResult, testCase.TestResult);
            }
        }, tc => !IsBusy);

        
        private void OnTestFinished(ITestResult testResult)
        {
            TestCycleResult.UpdateWith(testResult);
        }
    }
}
