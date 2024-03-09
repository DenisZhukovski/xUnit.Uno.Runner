using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace XUnit.Runners.Core;

/// <summary>
/// Custom Theory attribute which defaults to using the test method name for the DisplayName property
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("Xunit.Sdk.TheoryDiscoverer", "xunit.execution.{Platform}")]
public class TheoryAttribute : Xunit.TheoryAttribute
{
    public TheoryAttribute([CallerMemberName] string displayName = "")
    {
        base.DisplayName = displayName;
    }
}
