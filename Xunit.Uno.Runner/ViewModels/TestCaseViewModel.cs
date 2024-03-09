using CommunityToolkit.Mvvm.ComponentModel;
using Xunit.Abstractions;
using XUnit.Runners.Core;

namespace Xunit.Uno.Runner
{
	public class TestCaseViewModel : ObservableObject
	{
		TestResultViewModel _testResult;

		internal TestCaseViewModel(string assemblyFileName, ITestCase testCase)
		{
			AssemblyFileName = assemblyFileName ?? throw new ArgumentNullException(nameof(assemblyFileName));
			TestCase = testCase ?? throw new ArgumentNullException(nameof(testCase));
            TestResult = new TestResultViewModel(testCase);
		}

		public string AssemblyFileName { get; }

		public string DisplayName => TestCase.DisplayName;

		public ITestCase TestCase { get; }

		public TestResultViewModel TestResult
		{
			get => _testResult;
			private set => SetProperty(ref _testResult, value);
		}

        public Task RunAsync()
        {
            return Task.CompletedTask;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj)
                || (obj is ITestResult testResult && TestCase.Equals(testResult.Case))
                || TestCase.Equals(obj);
        }

        public override int GetHashCode()
        {
            return TestCase.GetHashCode();
        }
    }
}
