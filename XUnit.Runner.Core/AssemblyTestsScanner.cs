using System.Reflection;

namespace XUnit.Runners.Core;

public class AssemblyTestsScanner : ITestScanner
{
    public AssemblyTestsScanner(params Assembly[] assemblies)
        : this((IEnumerable<Assembly>)assemblies)
    {
    }
    
    public AssemblyTestsScanner(IEnumerable<Assembly> assemblies)
    {
        Assemblies = new List<Assembly>(assemblies);
    }

    public IReadOnlyCollection<Assembly> Assemblies { get; }
    
    public Task<IReadOnlyList<ITestCases>> ToListAsync(CancellationToken token)
    {
        return Task.FromResult<IReadOnlyList<ITestCases>>(
            Assemblies
                .Select(assembly => new AssemblyTestCases(assembly))
                .ToList()
        );
    }
}
