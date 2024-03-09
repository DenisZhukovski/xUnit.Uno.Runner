using System.Reflection;
using Xunit.Abstractions;

namespace XUnit.Runners.Core;

public class AssemblyTestCases(Assembly assembly) : ITestCases
{
    private readonly Assembly _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

    public string GroupName => _assembly.CrossPlatformLocation();

    /// <summary>
    /// Each time returns new test cycle
    /// </summary>
    public ITestCycle TestCycle => new AssemblyTestCycle(_assembly);

    public async Task<IReadOnlyList<ITestCase>> ToListAsync(CancellationToken token)
    {
        var testCases = await _assembly.TestCases(token);
        return testCases
            .Where(tc => tc.UniqueID != null)
            .ToList();
    }
}
