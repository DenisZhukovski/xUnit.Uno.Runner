using Dotnet.Commands;
using XUnit.Runners.Core.Log;

namespace Xunit.Uno.Runner;

public static class CommandsExtensions
{
    public static ICommands Logged(this ICommands commands, ILogger logger)
    {
        return new LoggedCommands(commands, logger);
    }
}
