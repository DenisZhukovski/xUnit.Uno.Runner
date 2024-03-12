using System.Collections.ObjectModel;
using XUnit.Runners.Core;

namespace Xunit.Uno.Runner.Extensions;

public static class AssemblyTestsExtensions
{
    public static async Task<ObservableCollection<TestCaseViewModel>> ToViewModels(this ITestCases assembly, CancellationToken token)
    {
        var testCases = await assembly.ToListAsync(token);
        return new ObservableCollection<TestCaseViewModel>(
            testCases.Select(
                testCase => new TestCaseViewModel(assembly.GroupName, testCase)
            )
        );
    }

    public static Task RunAsync(this IEnumerable<TestCaseViewModel> tests, CancellationToken token)
    {
        var progress = new List<Task>();
        foreach (var test in tests)
        {
            progress.Add(test.RunAsync(token));
        }

        return Task.WhenAll(progress);
    }
    
    public static Task RunAsync(
        this ObservableCollection<TestCasesViewModel> assemblies,
        CancellationToken token)
    {
        var progress = new List<Task>();
        foreach (var assembly in assemblies)
        {
            progress.Add(assembly.RunAllTestsCommand.ExecuteAsync(token));
        }

        return Task.WhenAll(progress);
    }
    
    public static void ClearResults(this IEnumerable<TestCaseViewModel> testCases)
    {
        foreach (var testCaseViewModel in testCases)
        {
            testCaseViewModel.TestResult.Clear();
        }
    }
}
