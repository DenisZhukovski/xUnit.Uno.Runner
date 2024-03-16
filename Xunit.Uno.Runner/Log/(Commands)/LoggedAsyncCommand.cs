using Dotnet.Commands;
using XUnit.Runners.Core.Log;

namespace Xunit.Uno.Runner;

public class LoggedAsyncCommand : IAsyncCommand
{
    private readonly IAsyncCommand _origin;
    private readonly ILogger _log;
    private readonly string? _name;

    public LoggedAsyncCommand(IAsyncCommand origin, ILogger log, string? name)
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
            _log.Log(
                LogLevel.Trace,
                "AsyncCommand {Name} {OnCanExecuteChangedName} subscribed", _name, nameof(CanExecuteChanged)
            );
        }

        remove
        {
            _origin.CanExecuteChanged -= value;
            _log.Log(
                LogLevel.Trace, "AsyncCommand {Name} {OnCanExecuteChangedName} unsubscribed", _name, nameof(CanExecuteChanged)
            );
        }
    }

    public void Cancel()
    {
        try
        {
            _origin.Cancel();
            _log.Log(LogLevel.Debug, "{Name} command cancelled", _name);
           
        }
        catch (Exception ex)
        {
            _log.LogError(0, ex, "{Name} cancel command failed", _name);
            throw;
        }
    }

    public bool CanExecute(object? parameter)
    {
        try
        {
            var result = _origin.CanExecute(parameter);
            _log.Log(
                LogLevel.Trace, 
                "{Name} {CanExecuteName} with parameter {Parameter} is {Result}", _name, nameof(CanExecute),
                parameter,
                result
            );
            return result;
        }
        catch (Exception ex)
        {
            _log.LogError(0, ex, "{Name} {CanExecuteName} with {Parameter} failed", _name, nameof(CanExecute), parameter);
            throw;
        }
    }

    public void Execute(object? parameter)
    {
        try
        {
            _origin.Execute(parameter);
            _log.Log(LogLevel.Debug, "{Name} with parameter {Parameter} executed", _name, parameter);
        }
        catch (Exception ex)
        {
            _log.LogError(0, ex, "{Name} with parameter {Parameter} execution failed", _name, parameter);
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
            
            _log.Log(LogLevel.Debug, "{Name} with parameter {Parameter} was executed: {Executed})", _name, parameter, executed);
            return executed;
        }
        catch (Exception ex)
        {
            _log.LogError(0, ex, "{Name} with parameter {Parameter} execution failed", _name, parameter);
            throw;
        }
    }

    public void RaiseCanExecuteChanged()
    {
        try
        {
            _origin.RaiseCanExecuteChanged();
            _log.Log(
                LogLevel.Trace,
                "{Name} {RaiseCanExecuteChangedName} called", _name, nameof(RaiseCanExecuteChanged)
            );
        }
        catch (Exception ex)
        {
            _log.LogError(0, ex, "{Name} RaiseCanExecuteChanged failed", _name);
            throw;
        }
    }
}

public class LoggedAsyncCommand<TParam> : IAsyncCommand<TParam>
{
    private readonly IAsyncCommand<TParam> _origin;
    private readonly ILogger _logger;
    private readonly string? _name;

    public LoggedAsyncCommand(IAsyncCommand<TParam> origin, ILogger logger, string? name)
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
                "AsyncCommand<TParam> {Name} CanExecuteChanged subscribed", _name
            );
        }

        remove
        {
            _origin.CanExecuteChanged -= value;
            _logger.Log(
                LogLevel.Trace, "AsyncCommand<TParam> {Name} CanExecuteChanged unsubscribed", _name
            );
        }
    }

    public void Cancel()
    {
        try
        {
            _origin.Cancel();
            _logger.Log(LogLevel.Debug, "{Name} command cancelled", _name);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "{Name} Cancel operation failed", _name);
            throw;
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
            _logger.Log(LogLevel.Debug, "{Name} Execute operation with parameter {Parameter} is successful", _name, parameter);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "{Name} Execute operation with parameter {Parameter} failed", _name, parameter);
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
            _logger.Log(LogLevel.Debug, "{Name} ExecuteAsync with parameter {Parameter} is finished (was executed: {Executed})", _name, parameter, executed);
            return executed;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "{Name} ExecuteAsync with parameter {Parameter} failed", _name, parameter);
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
            
            _logger.Log(
                LogLevel.Debug, 
                "{Name} ExecuteAsync operation with parameter {Parameter} is finished (was executed:{Executed})", _name, parameter, executed
            );
           
            return executed;
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "{Name} ExecuteAsync operation with parameter {Parameter} failed", _name, parameter);
            throw;
        }
    }

    public void RaiseCanExecuteChanged()
    {
        try
        {
            _origin.RaiseCanExecuteChanged();
            _logger.Log(
                LogLevel.Trace, 
                "{Name} RaiseCanExecuteChanged operation success", _name
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, "{Name} RaiseCanExecuteChanged failed", _name);
            throw;
        }
    }
}
