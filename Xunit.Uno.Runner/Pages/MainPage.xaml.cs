using Xunit.Uno.Runner.Navigation;

namespace Xunit.Uno.Runner;

public sealed partial class MainPage : Page
{
    private NavigationEventArgs? _onNavigatedToArgs;

    public MainPage()
    {
        this.InitializeComponent();
    }

    protected override void OnDataContextChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is INavigationAware navigationAware && _onNavigatedToArgs != null)
        {
            navigationAware.OnNavigatedTo(_onNavigatedToArgs);
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _onNavigatedToArgs = e;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        if (DataContext is INavigationAware navigationAware)
        {
            navigationAware.OnNavigatedFrom(e);
        }
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        (DataContext as MainViewModel).TestCasesCommand.Execute(e.AddedItems.First());
        AllTestsList.SelectedItem = null;
    }
}
