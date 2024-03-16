using System.Collections.ObjectModel;
using XUnit.Runners.Core.Log;

namespace Xunit.Uno.Runner;

public class DiagnosticViewModel : UIBindableBase, ILogger
{
    ObservableCollection<string> _messages = new();
    private LogLevel _logLevel = LogLevel.Error;

    public ObservableCollection<string> Messages
    {
        get => _messages;
        set => SetProperty(ref _messages, value);
    }

    public LogLevel LogLevel
    {
        get => _logLevel;
        set => SetProperty(ref _logLevel, value);
    }

    public IList<LogLevel> LogLevels => Enum.GetValues<LogLevel>();
    
    public void Clear()
    {
        Messages.Clear();
    }

    public void Write(string message)
    {
        this.Log(LogLevel.Information, message);
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }
        
        OnUIAsync(() => 
        {
            lock (Messages)
            {
                Messages.Add(formatter(state, exception));
                if (exception != null)
                {
                    Messages.Add(exception.Message);
                }
            }
            
        }).FireAndForget();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= LogLevel;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}
