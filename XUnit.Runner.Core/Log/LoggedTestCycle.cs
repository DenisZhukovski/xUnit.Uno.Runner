using Xunit.Abstractions;

namespace XUnit.Runners.Core.Log;

public class LoggedTestCycle : ITestCycle
{
    private readonly ITestCycle _origin;
    private readonly ILog _log;

    public LoggedTestCycle(ITestCycle origin, ILog log)
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
        try
        {
            _log.Write("TestCycle", $"Test cycle run started: '{testCases.Count}' tests");
            TestFinished += OnTestFinished;
            var result = await _origin.RunAsync(testCases, token);
            _log.Write("TestCycle", $"Test cycle run finished");
            return result;
        }
        catch (Exception ex)
        {
            _log.Write("TestCycle", $"Test Cycled run failed.", ex);
            throw;
        }
        finally
        {
            TestFinished -= OnTestFinished;
        }
    }

    private void OnTestFinished(ITestResult testResult)
    {
        _log.Write("TestCycle", $"Test finished: {testResult.Message}");
    }
}
