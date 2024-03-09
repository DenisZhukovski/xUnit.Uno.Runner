namespace XUnit.Runners.Core
{
	public interface ITestScanner
	{
		Task<IReadOnlyList<ITestCases>> ToListAsync(CancellationToken token);
	}
}
