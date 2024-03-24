using System.Collections.ObjectModel;

namespace XUnit.Runners.Core;

public class DelayedFilteredCollection<T> : IFilteredCollection<T>
{
    private readonly IFilteredCollection<T> _origin;
    private readonly TimeSpan _delay;
    CancellationTokenSource? _filterCancellation;

    public DelayedFilteredCollection(IFilteredCollection<T> origin)
        : this(origin, TimeSpan.FromMilliseconds(500))
    {
    }
    
    public DelayedFilteredCollection(IFilteredCollection<T> origin, TimeSpan delay)
    {
        _origin = origin;
        _delay = delay;
    }

    public int TotalCount => _origin.TotalCount;
    
    public ObservableCollection<T> List => _origin.List;

    public object? Filter
    {
        get => _origin.Filter;
        set
        {
            _filterCancellation?.Cancel();
            _filterCancellation = new CancellationTokenSource();
            if (TotalCount > 50)
            {
                Task.Delay(_delay, _filterCancellation.Token)
                    .ContinueWith(
                        x => _origin.Filter = value,
                        _filterCancellation.Token,
                        TaskContinuationOptions.None,
                        TaskScheduler.FromCurrentSynchronizationContext()
                    );
            }
            else
            {
                _origin.Filter = value;
            }
        }
    }
}
