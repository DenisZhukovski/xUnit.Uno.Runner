namespace Xunit.Uno.Runner.Navigation;

public interface INavigationAware
{
    void OnNavigatedTo(NavigationEventArgs e);

    void OnNavigatedFrom(NavigationEventArgs e);
}
