using Uno.Toolkit.UI;
using Xunit.Uno.Runner.Navigation;

namespace Xunit.Uno.Runner;

public sealed partial class MainPage : Page
{
    private NavigationEventArgs? _onNavigatedToArgs;
    
    public MainPage()
    {
        this.InitializeComponent();
        StatusBar.SetForeground(StatusBarForegroundTheme.Dark);
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
}
