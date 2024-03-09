using Dotnet.Commands;
using XUnit.Runners.Core.Log;

namespace Xunit.Uno.Runner;

public class LoggedAsyncCommand : IAsyncCommand
{
    private readonly IAsyncCommand _origin;
    private readonly ILog _log;
    private readonly string? _name;

    public LoggedAsyncCommand(IAsyncCommand origin, ILog log, string? name)
    {
        _origin = origin;
        _log = log;
        _name = name;
    }

    public event EventHandler? CanExecuteChanged
    {
        add
        {
            _origin.CanExecuteChanged += value;
            _log.Write("Command", $"AsyncCommand {_name} {nameof(CanExecuteChanged)} subscribed");
        }

        remove
        {
            _origin.CanExecuteChanged -= value;
            _log.Write("Command", $"AsyncCommand {_name} {nameof(CanExecuteChanged)} unsubscribed");
        }
    }

    public void Cancel()
    {
        try
        {
            _origin.Cancel();
            _log.Write("Command", $"{_name} command cancelled.");
        }
        catch (Exception ex)
        {
            _log.Write(
                "Command",
                $"{_name} cancel command failed.",
                ex
            );
            throw;
        }
    }

    public bool CanExecute(object? parameter)
    {
        try
        {
            var result = _origin.CanExecute(parameter);
            _log.Write("Command", $"{_name} {nameof(CanExecute)} with parameter {parameter} is {result}.");
            return result;
        }
        catch (Exception ex)
        {
            _log.Write(
                "Command",
                $"{_name} {nameof(CanExecute)} with {parameter} failed.",
                ex
            );
            throw;
        }
    }

    public void Execute(object? parameter)
    {
        try
        {
            _origin.Execute(parameter);
            _log.Write("Command", $"{_name} with parameter {parameter} executed.");
        }
        catch (Exception ex)
        {
            _log.Write(
                "Command",
                $"{_name} with parameter {parameter} execution failed.",
                ex
            );
            throw;
        }
    }

    public async Task<bool> ExecuteAsync(object? parameter)
    {
        try
        {
            var executed = await _origin
                .ExecuteAsync(parameter)
                .ConfigureAwait(false);
            _log.Write(
                "Command",
                $"{_name} with parameter {parameter} was executed: {executed})."
            );
            return executed;
        }
        catch (Exception ex)
        {
            _log.Write(
                "Command",
                $"{_name} with parameter {parameter} execution failed.",
                ex
            );
            throw;
        }
    }

    public void RaiseCanExecuteChanged()
    {
        try
        {
            _origin.RaiseCanExecuteChanged();
            _log.Write("Command", $"{_name} {nameof(RaiseCanExecuteChanged)} called.");
        }
        catch (Exception ex)
        {
            _log.Write(
                "Command", 
                $"{_name} RaiseCanExecuteChanged failed.",
                ex
            );
            throw;
        }
    }
}

public class LoggedAsyncCommand<TParam> : IAsyncCommand<TParam>
{
    private readonly IAsyncCommand<TParam> _origin;
    private readonly ILog _logger;
    private readonly string? _name;

    public LoggedAsyncCommand(IAsyncCommand<TParam> origin, ILog logger, string? name)
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
            _logger.Write("Command", $"AsyncCommand<TParam> {_name} CanExecuteChanged subscribed");
        }

        remove
        {
            _origin.CanExecuteChanged -= value;
            _logger.Write("Command", $"AsyncCommand<TParam> {_name} CanExecuteChanged unsubscribed");
        }
    }

    public void Cancel()
    {
        try
        {
            _origin.Cancel();
            _logger.Write("Command", $"{_name} Cancel operation success.");
        }
        catch (Exception ex)
        {
            _logger.Write(
                "Command", 
                $"{_name} Cancel operation failed.",
                ex
            );
            throw;
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
            _logger.Write(
                "Command",
                $"{_name} Check CanExecute with {parameter} failed.",
                ex
            );
            throw;
        }
    }

    public void Execute(object? parameter)
    {
        try
        {
            _origin.Execute(parameter);
            _logger.Write("Command", $"{_name} Execute operation with parameter {parameter} is successful.");
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

    public async Task<bool> ExecuteAsync(TParam? parameter)
    {
        try
        {
            var executed = await _origin
                .ExecuteAsync(parameter)
                .ConfigureAwait(false);
            _logger.Write(
                "Command",
                $"{_name} ExecuteAsync<TParam> with parameter {parameter} is finished (was executed: {executed})."
            );
            return executed;
        }
        catch (Exception ex)
        {
            _logger.Write(
                "Command",
                $"{_name} ExecuteAsync<TParam> operation with parameter {parameter} failed.",
                ex
            );
            throw;
        }
    }

    public async Task<bool> ExecuteAsync(object? parameter)
    {
        try
        {
            var executed = await _origin
                .ExecuteAsync(parameter)
                .ConfigureAwait(false);
            _logger.Write(
                "Command",
                $"{_name} ExecuteAsync operation with parameter {parameter} is finished (was executed:{executed})."
            );
            return executed;
        }
        catch (Exception ex)
        {
            _logger.Write(
                "Command",
                $"{_name} ExecuteAsync operation with parameter {parameter} failed.",
                ex
            );
            throw;
        }
    }

    public void RaiseCanExecuteChanged()
    {
        try
        {
            _origin.RaiseCanExecuteChanged();
            _logger.Write("Command", $"{_name} RaiseCanExecuteChanged operation success.");
        }
        catch (Exception ex)
        {
            _logger.Write(
                "Command",
                $"{_name} RaiseCanExecuteChanged failed.",
                ex
            );
            throw;
        }
    }
}
