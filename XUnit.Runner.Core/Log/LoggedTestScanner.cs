namespace XUnit.Runners.Core.Log;

public class LoggedTestScanner : ITestScanner
{
    private readonly ITestScanner _origin;
    private readonly ILog _log;

    public LoggedTestScanner(ITestScanner origin, ILog log)
    {
        _origin = origin;
        _log = log;
    }

    public async Task<IReadOnlyList<ITestCases>> ToListAsync(CancellationToken token)
    {
        var testsCasesList = await _origin.ToListAsync(token);
        return testsCasesList
            .Select(testCases => new LoggedTestCases(testCases, _log))
            .ToList();
    }
}
