using Xunit;
using Xunit.Abstractions;

namespace XUnit.Runners.Core;

public static class ExecutionEventSinkExtensions
{
    internal static Task<IReadOnlyList<ITestResult>> ExecuteAsync(
        this ExecutionEventSink execution,
        AssemblyTestCycle testCycle,
        Func<IReadOnlyList<ITestResult>> onExecuteAsync,
        CancellationToken token)
    {
        return execution.ExecuteAsync(
            testCycle,
            () => Task<IReadOnlyList<ITestResult>>.Factory.StartNew(
                onExecuteAsync,
                token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            )
        );
    }
    internal static async Task<IReadOnlyList<ITestResult>> ExecuteAsync(
        this ExecutionEventSink execution,
        AssemblyTestCycle testCycle,
        Func<Task<IReadOnlyList<ITestResult>>> onExecuteAsync)
    {
        void OnFailed(MessageHandlerArgs<ITestFailed> args) => testCycle.OnTestResult(args.Message, TestState.Failed);
        void OnPassed(MessageHandlerArgs<ITestPassed> args) => testCycle.OnTestResult(args.Message, TestState.Passed);
        void OnSkipped(MessageHandlerArgs<ITestSkipped> args) => testCycle.OnTestResult(args.Message, TestState.Skipped);
        
        execution.TestFailedEvent += OnFailed;
        execution.TestPassedEvent += OnPassed;
        execution.TestSkippedEvent += OnSkipped;
        try
        {
            return await onExecuteAsync();
        }
        finally
        {
            execution.TestFailedEvent -= OnFailed;
            execution.TestPassedEvent -= OnPassed;
            execution.TestSkippedEvent -= OnSkipped;
        }
    }
}
