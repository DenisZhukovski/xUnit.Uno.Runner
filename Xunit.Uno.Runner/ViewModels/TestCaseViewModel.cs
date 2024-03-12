using CommunityToolkit.Mvvm.ComponentModel;
using Xunit.Abstractions;
using XUnit.Runners.Core;

namespace Xunit.Uno.Runner
{
	public class TestCaseViewModel : ObservableObject
	{
		internal TestCaseViewModel(string assemblyFileName, ITestCase testCase)
		{
			AssemblyFileName = assemblyFileName ?? throw new ArgumentNullException(nameof(assemblyFileName));
			TestCase = testCase ?? throw new ArgumentNullException(nameof(testCase));
            TestResult = new TestResultViewModel(testCase);
		}

		public string AssemblyFileName { get; }

		public string DisplayName => TestCase.DisplayName;

		public ITestCase TestCase { get; }

		public TestResultViewModel TestResult { get; }

        public Task RunAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj)
                || (obj is ITestResult testResult && TestCase.Equals(testResult.Case))
                || TestCase.Equals(obj)
                || obj is ITestFilter filter && Equals(filter);
        }

        public override int GetHashCode()
        {
            return TestCase.GetHashCode();
        }
        
        private bool Equals(ITestFilter filter)
        {
            if (filter.State != TestState.All && !TestResult.Equals(filter.State))
            {
                return false;
            }

            return
                string.IsNullOrWhiteSpace(filter.Name) ||
                DisplayName.Contains(filter.Name.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
