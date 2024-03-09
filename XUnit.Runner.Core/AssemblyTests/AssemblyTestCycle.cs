using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace XUnit.Runners.Core;

internal class AssemblyTestCycle(Assembly assembly) : TestMessageSink, ITestCycle
{
    private readonly Assembly _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
    private IReadOnlyList<ITestCase>? _testCases;
    private readonly List<ITestResult> _testResults = new();
    private string? _assemblyFileName;
    private ITestFrameworkExecutionOptions? _options;

    public event Action<ITestResult>? TestFinished;

    private ITestFrameworkExecutionOptions Options => _options ??= TestFrameworkOptions.ForExecution(_assembly.TestConfiguration());
    
    private string AssemblyFileName => _assemblyFileName ??= _assembly.CrossPlatformLocation();
    
    public Task<IReadOnlyList<ITestResult>> RunAsync(
        IReadOnlyList<ITestCase> testCases,
        CancellationToken token)
    {
        _testResults.Clear();
        _testCases = testCases;
        return Execution.ExecuteAsync(
            this,
            () =>
            {
                OnStart();
                var summary = ExecuteTestCycle(testCases, token);
                OnFinish(summary);
                return _testResults;
            },
            token
        );
    }
    
    internal void OnTestResult(ITestResultMessage testResultMessage, TestState state)
    {
        if (_testCases?.TryByResult(testResultMessage, out ITestCase testCase) ?? false)
        {
            var testResult = new TestResult(testCase, testResultMessage, state);
            _testResults.Add(testResult);
            TestFinished?.Invoke(testResult);
        }
    }

    private ExecutionSummary ExecuteTestCycle(IReadOnlyList<ITestCase> testCases, CancellationToken token)
    {
        using var controller = new XunitFrontController(AppDomainSupport.Denied, AssemblyFileName);
        var execution = TestsExecution(
            _assembly.TestConfiguration().LongRunningTestSecondsOrDefault,
            token
        );
        controller.RunTests(testCases, execution, Options);
        execution.Finished.WaitOne();
        return execution.ExecutionSummary;
    }
    
    private void OnFinish(ExecutionSummary summary)
    {
        OnMessage(
            new TestAssemblyExecutionFinished(
                new XunitProjectAssembly { AssemblyFilename = AssemblyFileName },
                Options,
                summary
            )
        );
    }

    private void OnStart()
    {
        OnMessage(
            new TestAssemblyExecutionStarting(
                new XunitProjectAssembly { AssemblyFilename = AssemblyFileName },
                _options
            )
        );
    }
    
    private IExecutionSink TestsExecution(int longRunningSeconds, CancellationToken token)
    {
        var options = new ExecutionSinkOptions
        {
            CancelThunk = () => token.IsCancellationRequested,
            DiagnosticMessageSink = this
        };
        if (longRunningSeconds > 0)
        {
            options.LongRunningTestTime = TimeSpan.FromSeconds(longRunningSeconds);
        }
        
        return new ExecutionSink(
            this,
            options
        );
    }
}
