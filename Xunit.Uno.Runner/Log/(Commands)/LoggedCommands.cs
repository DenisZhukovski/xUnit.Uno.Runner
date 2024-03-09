using System.Runtime.CompilerServices;
using Dotnet.Commands;
using XUnit.Runners.Core.Log;

namespace Xunit.Uno.Runner;

public class LoggedCommands : ICommands
{
    private readonly ICommands _origin;
    private readonly ILog _log;

    public LoggedCommands(ICommands origin, ILog log)
    {
        _origin = origin;
        _log = log;
    }

    public IAsyncCommand AsyncCommand(
        Func<CancellationToken, Task> execute,
        Func<bool>? canExecute = null,
        bool forceExecution = false,
        [CallerMemberName] string? name = null)
    {
        return new LoggedAsyncCommand(
            _origin.AsyncCommand(execute, canExecute, forceExecution, name),
            _log,
            name
        );
    }

    public IAsyncCommand<TParam> AsyncCommand<TParam>(
        Func<TParam?, CancellationToken, Task> execute,
        Func<TParam?, bool>? canExecute = null,
        bool forceExecution = false,
        [CallerMemberName] string? name = null)
    {
        return new LoggedAsyncCommand<TParam>(
            _origin.AsyncCommand(execute, canExecute, forceExecution, name),
            _log,
            name
        );
    }

    public IAsyncCommand<TParam> AsyncCommand<TParam>(
        Func<TParam?, CancellationToken, Task> execute,
        Func<TParam?, Task<bool>>? canExecute = null,
        bool forceExecution = false,
        [CallerMemberName] string? name = null)
    {
        return new LoggedAsyncCommand<TParam>(
            _origin.AsyncCommand(execute, canExecute, forceExecution, name),
            _log,
            name
        );
    }

    public ICommand Command(
        Action execute,
        Func<bool>? canExecute = null,
        bool forceExecution = false,
        [CallerMemberName] string? name = null)
    {
        return new LoggedCommand(
            _origin.Command(execute, canExecute, forceExecution, name),
            _log,
            name
        );
    }

    public ICommand Command<TParam>(
        Action<TParam> execute,
        Func<TParam, bool>? canExecute = null,
        bool forceExecution = false,
        [CallerMemberName] string? name = null)
    {
        return new LoggedCommand(
            _origin.Command(execute, canExecute, forceExecution, name),
            _log,
            name
        );
    }
}
