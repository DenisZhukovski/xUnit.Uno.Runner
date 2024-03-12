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
        OnUIAsync(
            () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))
        ).FireAndForget();
    }

    protected async Task OnUIAsync(DispatcherQueueHandler callback)
    {
        var hasThreadAccess =
    #if __WASM__
        true;
    #else
        UIThread.HasThreadAccess;
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
                var enqueue = UIThread.TryEnqueue(() =>
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
