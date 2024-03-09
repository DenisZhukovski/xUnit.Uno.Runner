using Xunit.Abstractions;

namespace XUnit.Runners.Core.Log;

public class LoggedTestCases : ITestCases
{
    private readonly ITestCases _origin;
    private readonly ILog _log;

    public LoggedTestCases(ITestCases origin, ILog log)
    {
        _origin = origin;
        _log = log;
    }

    public string GroupName => _origin.GroupName;

    public ITestCycle TestCycle => new LoggedTestCycle(_origin.TestCycle, _log);

    public Task<IReadOnlyList<ITestCase>> ToListAsync(CancellationToken token)
    {
        return _origin.ToListAsync(token);
    }
}
