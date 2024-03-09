using CommunityToolkit.Mvvm.ComponentModel;
using Xunit.Abstractions;
using XUnit.Runners.Core;

namespace Xunit.Uno.Runner
{
	public class TestResultViewModel : ObservableObject
	{
		private TimeSpan _duration;
		private string? _message;
		private string? _stackTrace;
        string? _output;
        TestState _state;
        private RunStatus _runStatus;

        public TestResultViewModel(ITestCase testCase)
		{
			TestCase = testCase ?? throw new ArgumentNullException(nameof(testCase));
            State = TestState.NotRun;
            RunStatus = RunStatus.NotRun;
            Message = "🔷 not run";
		}

		public ITestCase TestCase { get; }

        public RunStatus RunStatus
        {
            get => _runStatus;
            set => SetProperty(ref _runStatus, value);
        }
        
		public TimeSpan Duration
		{
			get => _duration;
            set => SetProperty(ref _duration, value);
		}

		public string? Message
		{
			get => _message;
			set => SetProperty(ref _message, value);
		}

		public string? StackTrace
		{
			get => _stackTrace;
			set => SetProperty(ref _stackTrace, value);
		}

		public bool HasOutput => !string.IsNullOrWhiteSpace(Output);

        public string? Output
        {
            get => _output;
            private set => SetProperty(ref _output, value);
        }

        public TestState State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }
        
        internal void UpdateTestState(ITestResult result)
        {
            Output = result.Message.Output ?? string.Empty;

            if (result.Message is ITestPassed)
            {
                State = TestState.Passed;
                Message = $"✔ Success! {result.Duration.TotalMilliseconds} ms";
                RunStatus = RunStatus.Ok;
            }
            else if (result.Message is ITestFailed failedMessage)
            {
                State = TestState.Failed;
                Message = $"⛔ {ExceptionUtility.CombineMessages(failedMessage)}";
                StackTrace = ExceptionUtility.CombineStackTraces(failedMessage);
                RunStatus = RunStatus.Failed;
            }
            else if (result.Message is ITestSkipped skipped)
            {
                State = TestState.Skipped;
                Message = $"⚠ {skipped.Reason}";
                RunStatus = RunStatus.Skipped;
            }
            else
            {
                Message = string.Empty;
                StackTrace = string.Empty;
                RunStatus = RunStatus.NotRun;
            }
        }
	}
}
