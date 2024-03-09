namespace XUnit.Runners.Core.Log;

public interface ILog
{
    void Write(string tag, string message);
    
    void Write(string tag, string message, Exception exception);
}