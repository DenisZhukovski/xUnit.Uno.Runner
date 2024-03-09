using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace XUnit.Runners.Core;

/// <summary>
/// Custom Fact attribute which defaults to using the test method name for the DisplayName property
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("Xunit.Sdk.FactDiscoverer", "xunit.execution.{Platform}")]
public class FactAttribute : Xunit.FactAttribute
{
    public FactAttribute([CallerMemberName] string displayName = "")
    {
        base.DisplayName = displayName;
    }
}
