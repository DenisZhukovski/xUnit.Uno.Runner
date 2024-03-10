namespace Xunit.Uno.Runner;

public static class TaskExtensions
{
    public static async void FireAndForget(this Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
    }
}
