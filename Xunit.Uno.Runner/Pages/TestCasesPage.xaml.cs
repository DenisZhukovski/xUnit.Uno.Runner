namespace Xunit.Uno.Runner;

public partial class TestCasesPage : Page
{
    public TestCasesPage()
    {
        InitializeComponent();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Any())
        {
            (DataContext as TestCasesViewModel).NavigateToResultCommand.Execute(e.AddedItems.First());
        }
        TestCasesList.SelectedItems.Clear();
    }
}
