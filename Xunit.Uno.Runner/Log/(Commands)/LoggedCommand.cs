using XUnit.Runners.Core.Log;

namespace Xunit.Uno.Runner;

public class LoggedCommand : ICommand
{
    private readonly ICommand _origin;
    private readonly ILog _logger;
    private readonly string? _name;

    public LoggedCommand(ICommand origin, ILog logger, string? name)
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
            _logger.Write("Command", $"Command {_name} CanExecuteChanged subscribed");
        }

        remove
        {
            _origin.CanExecuteChanged -= value;
            _logger.Write("Command", $"Command {_name} CanExecuteChanged unsubscribed");
        }
    }

    public bool CanExecute(object? parameter)
    {
        try
        {
            var result = _origin.CanExecute(parameter);
            _logger.Write("Command", $"{_name} Check CanExecute with parameter {parameter} is {result}.");
            return result;
        }
        catch (Exception ex)
        {
            _logger.Write("Command", $"{_name} Check CanExecute with {parameter} failed: {ex.Message}.");
            throw;
        }
    }

    public void Execute(object? parameter)
    {
        try
        {
            _origin.Execute(parameter);
            _logger.Write("Command", $"{_name} Execute operation with parameter {parameter} success.");
        }
        catch (Exception ex)
        {
            _logger.Write(
                "Command",
                $"{_name} Execute operation with parameter {parameter} failed.",
                ex
            );
            throw;
        }
    }
}
