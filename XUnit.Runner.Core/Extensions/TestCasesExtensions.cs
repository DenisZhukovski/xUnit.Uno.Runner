using Xunit.Abstractions;

namespace XUnit.Runners.Core;

public static class TestCasesExtensions
{
    public static async Task<IReadOnlyList<ITestResult>> RunAsync(
        this ITestCases testCases,
        CancellationToken token)
    {
        return await testCases
            .TestCycle
            .RunAsync(
                await testCases.ToListAsync(token),
                token
            );
    }
    
    public static ITestCase? ByResult(this IReadOnlyList<ITestCase> testCases, ITestResultMessage testResult)
    {
        var testCase = testCases.FirstOrDefault(tc => tc.Equals(testResult.TestCase));
        if (testCase == null)
        {
            // no matching reference, search by Unique ID as a fallback
            testCase = testCases.FirstOrDefault(
                tc => tc.UniqueID?.Equals(testResult.TestCase.UniqueID, StringComparison.Ordinal) ?? false
            );
        }
        
        return testCase;
    }

    public static bool TryByResult(
        this IReadOnlyList<ITestCase> testCases,
        ITestResultMessage testResult,
        out ITestCase testCase)
    {
        _ = testCases ?? throw new ArgumentNullException(nameof(testCases));
        testCase = testCases.ByResult(testResult);
        return testCase != null;
    }
}
