namespace XUnit.Runners.Core;

public struct TestFilter : ITestFilter
{
    public TestFilter()
    {
    }

    public string Name { get; set; } = string.Empty;

    public TestState State { get; set; } = TestState.All;
}
