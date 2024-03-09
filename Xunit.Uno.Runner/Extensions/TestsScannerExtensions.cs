using System.Collections.ObjectModel;
using Dotnet.Commands;
using XUnit.Runners.Core;

namespace Xunit.Uno.Runner.Extensions;

public static class TestsScannerExtensions
{
    public static async Task<ObservableCollection<TestCasesViewModel>> ToViewModels(
        this ITestScanner scanner,
        INavigator navigator,
        ICommands commands,
        CancellationToken token)
    {
        var testCasesList = await scanner.ToListAsync(token);
        return new ObservableCollection<TestCasesViewModel>(
            testCasesList.Select(
                testCases => new TestCasesViewModel(testCases, navigator, commands)
            )
        );
    }
}
