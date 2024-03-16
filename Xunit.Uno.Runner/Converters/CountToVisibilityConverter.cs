using System.Collections;
using Microsoft.UI.Xaml.Data;
using Uno.Extensions.Specialized;

namespace Xunit.Uno.Runner.UX;

public class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int count)
        {
            return AsVisibility(count > 0, parameter);
        }
        
        if (value is IList list)
        {
            return AsVisibility(list.Count > 0, parameter);
        }
        
        if (value is IEnumerable enumerable)
        {
            return AsVisibility(enumerable.Any(), parameter);
        }

        return Visibility.Collapsed;
    }

    private Visibility AsVisibility(bool @true, object parameter)
    {
        if (parameter?.ToString() == "Inverse")
        {
            return @true ? Visibility.Collapsed : Visibility.Visible; 
        }
        
        return @true ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
