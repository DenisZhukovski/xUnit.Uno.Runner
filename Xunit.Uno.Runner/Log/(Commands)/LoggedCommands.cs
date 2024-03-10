using System.Runtime.CompilerServices;
using Dotnet.Commands;
using XUnit.Runners.Core.Log;

namespace Xunit.Uno.Runner;

public class LoggedCommands : ICommands
{
    private readonly ICommands _origin;
    private readonly ILog _log;
    private readonly bool _fullLog;

    public LoggedCommands(ICommands origin, ILog log, bool fullLog = true)
    {
        _origin = origin;
        _log = log;
        _fullLog = fullLog;
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
            name,
            _fullLog
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
            name,
            _fullLog
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
            name,
            _fullLog
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
            name,
            _fullLog
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
            name,
            _fullLog
        );
    }
}
