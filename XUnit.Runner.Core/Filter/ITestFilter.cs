namespace XUnit.Runners.Core;

public interface ITestFilter
{
    string Name { get; set; }
    
    TestState State { get; set; }
}
