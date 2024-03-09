using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using XUnit.Runners.Core;

namespace Xunit.Uno.Runner.UX
{
	class RunStatusToColorConverter : IValueConverter
	{
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not RunStatus status || Application.Current == null)
                return Colors.Red;

            return status switch
            {
                RunStatus.Ok => Application.Current.Resources["VisualRunnerSuccessfulTestsColor"],
                RunStatus.Failed => Application.Current.Resources["VisualRunnerFailedTestsColor"],
                RunStatus.NoTests => Application.Current.Resources["VisualRunnerNoTestsColor"],
                RunStatus.NotRun => Application.Current.Resources["VisualRunnerNotRunTestsColor"],
                RunStatus.Skipped => Application.Current.Resources["VisualRunnerSkippedTestsColor"],
                _ => throw new ArgumentOutOfRangeException(nameof(value)),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
