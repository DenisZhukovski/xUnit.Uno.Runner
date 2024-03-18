
using Microsoft.UI;

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

    private void ListViewBase_OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if ((args.ItemIndex + 1) % 2 == 0)
        {
            args.ItemContainer.Background = new SolidColorBrush(Colors.FromARGB(255, 247, 247, 247));
        }
    }
}
