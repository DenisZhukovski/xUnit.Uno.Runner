using Microsoft.UI.Dispatching;

namespace Xunit.Uno.Runner;

public static class UIThreadExtensions
{
    public static async Task OnUIAsync(this DispatcherQueue uiThread, DispatcherQueueHandler callback)
    {
        var hasThreadAccess =
#if __WASM__
        true;
#else
            uiThread.HasThreadAccess;
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
                var enqueue = uiThread.TryEnqueue(() =>
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
