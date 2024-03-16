using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace XUnit.Runners.Core.Log;

public class LoggedTestCycle : ITestCycle
{
    private readonly ITestCycle _origin;
    private readonly ILogger _log;

    public LoggedTestCycle(ITestCycle origin, ILogger log)
    {
        _origin = origin;
        _log = log;
    }

    public event Action<ITestResult>? TestFinished
    {
        add => _origin.TestFinished += value;
        remove => _origin.TestFinished -= value;
    }

    public async Task<IReadOnlyList<ITestResult>> RunAsync(IReadOnlyList<ITestCase> testCases, CancellationToken token)
    {
        int count = testCases.Count;
        void OnTestFinished(ITestResult testResult)
        {
            count--;
            _log.Log(LogLevel.Debug, "Test finished: {TestResult}", testResult);
            if (count == 0)
            {
                _log.Log(LogLevel.Information, "Test cycle run finished");
            }
        }
        
        try
        {
            _log.Log(LogLevel.Information, "Test cycle run started: '{TestCasesCount}' tests", testCases.Count);
            TestFinished += OnTestFinished;
            return await _origin.RunAsync(testCases, token);
        }
        catch (Exception ex)
        {
            _log.LogError(0, ex, "Test Cycled run failed");
            throw;
        }
        finally
        {
            TestFinished -= OnTestFinished;
        }
    }
}
