using System.Collections.ObjectModel;
using XUnit.Runners.Core.Log;

namespace Xunit.Uno.Runner;

public class DiagnosticViewModel : DispatchedBindableBase, ILog
{
    ObservableCollection<string> _messages = new ObservableCollection<string>();
    
    public ObservableCollection<string> Messages
    {
        get => _messages;
        set => SetProperty(ref _messages, value);
    }

    public void Clear()
    {
        Messages.Clear();
    }

    public void Write(string message)
    {
        Write(string.Empty, message);
    }

    public void Write(string tag, object message)
    {
        if (message == null)
        {
            return;
        }
        
        DispatchAsync(() =>
        {
            lock (Messages)
            {
                Messages.Add(message.ToString());
            }
        }).FireAndForget();
    }

    public void Write(string tag, string message, Exception exception)
    {
        DispatchAsync(() => 
        {
            lock (Messages)
            {
                Messages.Add(message);
                Messages.Add(exception.Message);
            }
            
        }).FireAndForget();
       
    }
}
