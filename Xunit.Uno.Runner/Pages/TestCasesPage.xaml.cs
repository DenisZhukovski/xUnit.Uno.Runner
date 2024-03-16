namespace Xunit.Uno.Runner;

public partial class TestCasesPage : Page
{
    public TestCasesPage()
    {
        InitializeComponent();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PrimaryButton.Focus(FocusState.Programmatic);
    }
}
