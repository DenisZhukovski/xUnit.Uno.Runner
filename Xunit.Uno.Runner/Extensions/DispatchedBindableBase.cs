using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Dispatching;

namespace Xunit.Uno.Runner;

public abstract class DispatchedBindableBase : INotifyPropertyChanged
{
    private DispatcherQueue? _dispatcher = null;

    // Insert variables below here
    protected DispatcherQueue Dispatcher => _dispatcher ??= DispatcherQueue.GetForCurrentThread();

    // Insert variables below here
    public event PropertyChangedEventHandler? PropertyChanged;

    // Insert SetProperty below here
    protected virtual bool SetProperty<T>(ref T backingVariable, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingVariable, value)) return false;

        backingVariable = value;
        RaisePropertyChanged(propertyName);

        return true;
    }

    // Insert RaisePropertyChanged below here
    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        DispatchAsync(
            () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))
        ).FireAndForget();
    }

    // Insert DispatchAsync below here
    protected async Task DispatchAsync(DispatcherQueueHandler callback)
    {
        var hasThreadAccess =
    #if __WASM__
        true;
    #else
        Dispatcher.HasThreadAccess;
    #endif

        if (hasThreadAccess)
        {
            callback.Invoke();
        }
        else
        {
            var completion = new TaskCompletionSource();
            int count = 20;
            while (count > 0)
            {
                var enqueue = Dispatcher.TryEnqueue(() =>
                {
                    callback();
                    completion.SetResult();
                });
                if (enqueue)
                {
                    break;
                }
                count--;
                await Task.Yield();
            }
            
            await completion.Task; 
        }
    }
}
