using CommunityToolkit.Mvvm.ComponentModel;
using XUnit.Runners.Core.Log;

namespace Xunit.Uno.Runner;

public class DiagnosticViewModel : ObservableObject, ILog
{
    string _messages = string.Empty;
    
    public string Messages
    {
        get => _messages;
        set
        {
            SynchronizationContext.Current.Post(_ => SetProperty(ref _messages, value), null);
        }
    }

    public void Clear()
    {
        Messages = string.Empty;
    }

    public void Write(string tag, string message)
    {
        Messages += $"{message}{Environment.NewLine}{Environment.NewLine}";
    }

    public void Write(string tag, string message, Exception exception)
    {
        Messages += $"{message}{Environment.NewLine}{exception.Message}{Environment.NewLine}{Environment.NewLine}";
    }
}
