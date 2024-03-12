using System.Reflection;

namespace XUnit.Runners.Core;

public class AssemblyTestsScanner(IEnumerable<Assembly> assemblies) : ITestScanner
{
    public AssemblyTestsScanner(params Assembly[] assemblies)
        : this((IEnumerable<Assembly>)assemblies)
    {
    }

    public Task<IReadOnlyList<ITestCases>> ToListAsync(CancellationToken token)
    {
        return Task.FromResult<IReadOnlyList<ITestCases>>(
            assemblies
                .Select(assembly => new AssemblyTestCases(assembly))
                .ToList()
        );
    }
}
