using System.Collections.ObjectModel;
using Dotnet.Commands;
using XUnit.Runners.Core;
using Xunit.Uno.Runner.Extensions;

namespace Xunit.Uno.Runner
{
	public class TestCasesViewModel : DispatchedBindableBase
	{
		private readonly ObservableCollection<TestCaseViewModel> _allTests = new();
        private readonly FilteredCollectionView<TestCaseViewModel, (string, TestState)> _filteredTests;
        private readonly ITestCases _testCases;
        readonly INavigator _navigation;
        private readonly ICommands _commands;

        CancellationTokenSource? _filterCancellationTokenSource;
		
		string? _searchQuery;
		
        private CancellationToken? _progressCancelToken;
        
        internal TestCasesViewModel(
            ITestCases testCases,
            INavigator navigation,
            ICommands commands)
		{
            _testCases = testCases;
            _navigation = navigation;
            _commands = commands.Cached();

            TestCycleResult = new TestCycleResultViewModel(_allTests);
            _filteredTests = new FilteredCollectionView<TestCaseViewModel, (string, TestState)>(
                _allTests,
                IsTestFilterMatch,
                (SearchQuery, TestCycleResult.ResultFilter),
                new TestComparer()
            );

            _filteredTests.ItemChanged += (_, _) => TestCycleResult.UpdateCaption();
            _filteredTests.CollectionChanged += (_, _) => TestCycleResult.UpdateCaption();
            
            InitCommand.Execute();
		}

        private CancellationToken? ProgressCancelToken
        {
            get => _progressCancelToken;
            set
            {
                if (_progressCancelToken != value)
                {
                    _progressCancelToken = value;
                    RunAllTestsCommand.RaiseCanExecuteChanged();
                    RunFilteredTestsCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(nameof(IsBusy));
                }
            }
        }
        
        public string DisplayName => Path.GetFileNameWithoutExtension(_testCases.GroupName);
        
        public string SearchQuery
        {
            get => _searchQuery ?? string.Empty;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    FilterAfterDelay();
                }
            }
        }
        
        public TestCycleResultViewModel TestCycleResult { get; }

        public IList<TestCaseViewModel> TestCases => _filteredTests;

        public bool IsBusy => ProgressCancelToken != null;
        
        public IAsyncCommand InitCommand => _commands.AsyncCommand(async token =>
        {
            try
            {
                ProgressCancelToken = token;
                var tests = await _testCases.ToViewModels(token);
                await DispatchAsync(() => _allTests.ReplaceWith(tests));
                TestCycleResult.UpdateCaption();
            }
            finally
            {
                ProgressCancelToken = null;
            }
        });
        
        public IAsyncCommand RunAllTestsCommand => _commands.AsyncCommand(async token =>
        {
            ITestCycle? testCycle = null;
            try
            {
                ProgressCancelToken = token;
                foreach (var testCaseViewModel in _allTests)
                {
                    testCaseViewModel.TestResult.Clear();
                }
                TestCycleResult.UpdateCaption();
                testCycle = _testCases.TestCycle;
                testCycle.TestFinished += OnTestFinished;
                await testCycle.RunAsync(
                    TestCases.Select(tc => tc.TestCase).ToList(),
                    token
                );
            }
            finally
            {
                ProgressCancelToken = null;
                if (testCycle != null)
                {
                    testCycle.TestFinished -= OnTestFinished;
                }
            }

        }, () => !IsBusy);

        public IAsyncCommand RunFilteredTestsCommand => _commands.AsyncCommand(async (token) =>
        {
            try
            {
                ProgressCancelToken = token;
                await _filteredTests.RunAsync();
            }
            finally
            {
                ProgressCancelToken = null;
            }

        }, () => !IsBusy);

        public ICommand NavigateToResultCommand => _commands.AsyncCommand<TestCaseViewModel?>(async testCase =>
        {
            if (testCase != null)
            {
                await testCase.RunAsync();
                //await _navigation.NavigateAsync(PageType.TestResult, testCase.TestResult);
            }
        }, tc => !IsBusy);

        private void OnTestFinished(ITestResult testResult)
        {
            _allTests
                .First(test => test.Equals(testResult))
                .TestResult.UpdateTestState(testResult);
            TestCycleResult.UpdateCaption();
        }
        
		void FilterAfterDelay()
		{
			_filterCancellationTokenSource?.Cancel();
			_filterCancellationTokenSource = new CancellationTokenSource();

			var token = _filterCancellationTokenSource.Token;

			Task.Delay(500, token)
				.ContinueWith(
					x => { _filteredTests.FilterArgument = (SearchQuery, TestCycleResult.ResultFilter); },
					token,
					TaskContinuationOptions.None,
					TaskScheduler.FromCurrentSynchronizationContext()
                );
		}

		static bool IsTestFilterMatch(TestCaseViewModel test, (string SearchQuery, TestState ResultFilter) query)
		{
			if (test == null)
				throw new ArgumentNullException(nameof(test));

			var (pattern, state) = query;

			TestState? requiredTestState = state switch
			{
				TestState.All => null,
				TestState.Passed => TestState.Passed,
				TestState.Failed => TestState.Failed,
				TestState.Skipped => TestState.Skipped,
				TestState.NotRun => TestState.NotRun,
				_ => throw new ArgumentException(),
			};

            if (requiredTestState.HasValue && test.TestResult.State != requiredTestState.Value)
            {
                return false;
            }

			return
				string.IsNullOrWhiteSpace(pattern) ||
				test.DisplayName.IndexOf(pattern.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
		}

		class TestComparer : IComparer<TestCaseViewModel>
		{
			public int Compare(TestCaseViewModel? x, TestCaseViewModel? y) =>
				string.Compare(x?.DisplayName, y?.DisplayName, StringComparison.OrdinalIgnoreCase);
		}
    }
}
