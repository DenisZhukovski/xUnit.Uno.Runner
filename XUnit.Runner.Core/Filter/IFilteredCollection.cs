using System.Collections.ObjectModel;

namespace XUnit.Runners.Core;

public interface IFilteredCollection<T>
{
    ObservableCollection<T> List { get; }

    object? Filter { get; set; }

    int TotalCount { get; }
}
