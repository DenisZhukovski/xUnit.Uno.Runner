using Microsoft.Extensions.Logging;
using XUnit.Runners.Core.Log;

namespace XUnit.Runners.Core;

public static class TestScannerExtensions
{
    public static ITestScanner Logged(this ITestScanner scanner, ILogger log)
    {
        return new LoggedTestScanner(scanner, log);
    }
}
