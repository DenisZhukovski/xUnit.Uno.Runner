using Xunit.Abstractions;

namespace XUnit.Runners.Core;

public interface ITestCases
{
    string GroupName { get; }
    
    ITestCycle TestCycle { get; }
    
    Task<IReadOnlyList<ITestCase>> ToListAsync(CancellationToken token);
}
