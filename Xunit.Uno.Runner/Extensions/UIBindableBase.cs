using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Dispatching;

namespace Xunit.Uno.Runner;

public abstract class UIBindableBase : INotifyPropertyChanged
{
    private DispatcherQueue? _dispatcher;

    protected DispatcherQueue UIThread => _dispatcher ??= DispatcherQueue.GetForCurrentThread();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual bool SetProperty<T>(
        ref T backingVariable,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingVariable, value)) return false;

        backingVariable = value;
        RaisePropertyChanged(propertyName);

        return true;
    }

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        UIThread.OnUIAsync(
            () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))
        ).FireAndForget();
    }
}
