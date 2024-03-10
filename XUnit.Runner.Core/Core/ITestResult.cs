using Xunit;
using Xunit.Abstractions;

namespace XUnit.Runners.Core;

public interface ITestResult
{
    ITestCase Case { get; }
    
    ITestResultMessage Message { get; }
    
    TimeSpan Duration { get; }
    
    string ErrorMessage { get; }
    
    string ErrorStackTrace { get; }
}

public readonly struct TestResult(ITestCase @case, ITestResultMessage message, TestState state) : ITestResult
{
    public ITestCase Case { get; } = @case;

    public ITestResultMessage Message { get; } = message;
    
    public TimeSpan Duration => TimeSpan.FromSeconds((double)Message.ExecutionTime);

    public string ErrorMessage => state == TestState.Failed
        ? ExceptionUtility.CombineMessages((ITestFailed)Message)
        : string.Empty;
    
    public string ErrorStackTrace => state == TestState.Failed
        ? ExceptionUtility.CombineStackTraces((ITestFailed)Message)
        : string.Empty;

    public override string ToString()
    {
        var index = Case.DisplayName.LastIndexOf(".") + 1;
        var name = Case.DisplayName.Substring(index,  Case.DisplayName.Length - index);
        return $"{name}:{state}";
    }
}
