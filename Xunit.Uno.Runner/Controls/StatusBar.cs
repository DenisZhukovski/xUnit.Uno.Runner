using Windows.UI.ViewManagement;
using Microsoft.UI;

namespace Xunit.Uno.Runner.UX;

public class StatusBar
{
    private readonly UISettings _uiSettings;
#if __ANDROID__
    private bool? _wasDarkMode = null;
#endif

    public StatusBar()
        : this(new UISettings())
    {
    }
    
    public StatusBar(UISettings uiSettings)
    {
        _uiSettings = uiSettings ?? throw new ArgumentNullException(nameof(uiSettings));
        _uiSettings.ColorValuesChanged += (s, e) =>
        {
#if __ANDROID__
            var backgroundColor = _uiSettings.GetColorValue(UIColorType.Background);
            var isDarkMode = backgroundColor == Colors.Black;

            // Prevent deadlock as setting StatusBar.ForegroundColor will also trigger this event.
            if (_wasDarkMode == isDarkMode)
            {
                return;
            }
            _wasDarkMode = isDarkMode;
#endif

            UpdateStatusBar();
        };

#if __IOS__
        // Force an update when the app is launched.
        UpdateStatusBar();
#endif
    }
    
    public void UpdateStatusBar()
    {
        // === 1. Determine the current theme from the background value,
        // which is calculated from the theme and can only be black or white.
        var backgroundColor = _uiSettings.GetColorValue(UIColorType.Background);
        var isDarkMode = backgroundColor == Colors.Black;

#if __IOS__ || __ANDROID__
        // === 2. Set the foreground color.
        // note: The foreground color can only be set to a "dark/light" value. See uno remarks on StatusBar.ForegroundColor.
        // note: For ios in dark mode, setting this value will have no effect.
        var foreground = isDarkMode ? Colors.White : Colors.Black;
        Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = foreground;

        // === 3. Set the background color.
        var background = isDarkMode ? Colors.MidnightBlue : Colors.SkyBlue;
#if __ANDROID__
        // On Android, this is done by calling Window.SetStatusBarColor.
        if (ContextHelper.Current is Android.App.Activity activity)
        {
            activity.Window.SetStatusBarColor(background);
        }
#endif
        // On iOS, this is done via the native CommandBar which goes under the status bar.
        // For android, we will also update the CommandBar just for consistency.
        // if (MainPage.Instance.GetCommandBar() is { } commandBar)
        // {
        //     commandBar.Foreground = new SolidColorBrush(foreground); // controls the color for the "MainPage" page title
        //     commandBar.Background = new SolidColorBrush(background);
        // }
#endif
    }
}
