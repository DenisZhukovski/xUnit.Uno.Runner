using XUnit.Runners.Core.Log;

namespace Xunit.Uno.Runner;

public class LoggedCommand : ICommand
{
    private readonly ICommand _origin;
    private readonly ILogger _logger;
    private readonly string? _name;

    public LoggedCommand(ICommand origin, ILogger logger, string? name)
    {
        _origin = origin;
        _logger = logger;
        _name = name;
    }

    public event EventHandler? CanExecuteChanged
    {
        add
        {
            _origin.CanExecuteChanged += value;
            _logger.Log(
                LogLevel.Trace, 
                "Command {Name} CanExecuteChanged subscribed", _name
            );
        }

        remove
        {
            _origin.CanExecuteChanged -= value;
            _logger.Log(
                LogLevel.Trace,
                "Command {Name} CanExecuteChanged unsubscribed", _name
            );
        }
    }

    public bool CanExecute(object? parameter)
    {
        try
        {
            var result = _origin.CanExecute(parameter);
            _logger.Log(
                LogLevel.Trace, 
                "{Name} Check CanExecute with parameter {Parameter} is {Result}", _name, parameter, result
            );
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "{Name} Check CanExecute with {Parameter} failed", _name, parameter);
            throw;
        }
    }

    public void Execute(object? parameter)
    {
        try
        {
            _origin.Execute(parameter);
            _logger.Log(LogLevel.Debug, "{Name} Execute operation with parameter {Parameter} success", _name, parameter);
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "{Name} Execute operation with parameter {Parameter} failed", _name, parameter);
            throw;
        }
    }
}
