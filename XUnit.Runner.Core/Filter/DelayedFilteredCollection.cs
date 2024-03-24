using System.Collections.ObjectModel;

namespace XUnit.Runners.Core;

public class DelayedFilteredCollection<T> : IFilteredCollection<T>
{
    private readonly IFilteredCollection<T> _origin;
    private readonly TimeSpan _delay;
    private readonly int _noDelayCount;
    CancellationTokenSource? _filterCancellation;

    public DelayedFilteredCollection(IFilteredCollection<T> origin, int noDelayCount = 50)
        : this(origin, TimeSpan.FromMilliseconds(500), noDelayCount)
    {
    }
    
    public DelayedFilteredCollection(IFilteredCollection<T> origin, TimeSpan delay, int noDelayCount = 50)
    {
        _origin = origin;
        _delay = delay;
        _noDelayCount = noDelayCount;
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
            if (TotalCount > _noDelayCount)
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
