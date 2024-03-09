using Xunit.Abstractions;

namespace XUnit.Runners.Core;

public interface ITestCycle
{
    event Action<ITestResult> TestFinished;
    
    Task<IReadOnlyList<ITestResult>> RunAsync(
        IReadOnlyList<ITestCase> testCases,
        CancellationToken token
    );
}
