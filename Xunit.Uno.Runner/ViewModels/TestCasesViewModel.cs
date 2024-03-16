using System.Collections.ObjectModel;
using Dotnet.Commands;
using Xunit.Abstractions;
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
            Filter = new TestsFilterViewModel(_filteredTests);
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
        
        public TestCycleResultViewModel TestCycleResult { get; }

        public TestsFilterViewModel Filter { get; }
        
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
        
        public IAsyncCommand RunAllTestsCommand => _commands.AsyncCommand(
            token =>
            {
                TestCycleResult.Clear();
                return RunTestCycle(
                    TestCases.Select(tc => tc.TestCase).ToList(),
                    token
                );
            },
            () => !IsBusy
        );
        
        public ICommand NavigateToResultCommand => _commands.AsyncCommand<TestCaseViewModel?>(
            async (testCase, token) =>
        {
            if (testCase != null)
            {
                await RunTestCycle(new[] { testCase.TestCase }, token);
                await _navigation.NavigateViewAsync<TestResultPage>(this, data: testCase);
            }
        }, tc => !IsBusy);

        private async Task RunTestCycle(
            IReadOnlyList<ITestCase> testCases,
            CancellationToken token)
        {
            ITestCycle? testCycle = null;
            try
            {
                Progress = token;
                testCycle = _testCases.TestCycle;
                testCycle.TestFinished += OnTestFinished;
                await testCycle.RunAsync(
                    testCases,
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
        }
        
        private void OnTestFinished(ITestResult testResult)
        {
            TestCycleResult.UpdateWith(testResult);
        }
    }
}
