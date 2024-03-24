using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.UI.Dispatching;
using XUnit.Runners.Core;

namespace Xunit.Uno.Runner;

public class LazyFilteredCollection<T> : IFilteredCollection<T>
{
    private readonly IFilteredCollection<T> _origin;
    private readonly DispatcherQueue _uiThread;
    private readonly Func<Task> _lazyTick;
    private readonly ObservableCollection<T> _lazy = new();

    public LazyFilteredCollection(IFilteredCollection<T> origin)
        : this(origin, DispatcherQueue.GetForCurrentThread())
    {
    }
    
    public LazyFilteredCollection(IFilteredCollection<T> origin, DispatcherQueue uiThread)
        : this(origin, uiThread, () => Task.Delay(25))
    {
    }
    
    public LazyFilteredCollection(
        IFilteredCollection<T> origin,
        DispatcherQueue uiThread,
        Func<Task> lazyTick)
    {
        _origin = origin;
        _uiThread = uiThread;
        _lazyTick = lazyTick;
        _origin.List.CollectionChanged += OnCollectionChanged;
    }

    public object? Filter
    {
        get => _origin.Filter;
        set => _origin.Filter = value;
    }

    public int TotalCount => _origin.TotalCount;
    
    public ObservableCollection<T> List
    {
        get
        {
            Reset();
            return _lazy;
        }
    }
    
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                _lazy.AddRange(e.NewItems ?? ReadOnlyCollection<T>.Empty);
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (T oldItem in e.OldItems ?? ReadOnlyCollection<T>.Empty)
                {
                    _lazy.Remove(oldItem);
                }
               
                break;
            case NotifyCollectionChangedAction.Replace:
                foreach (T oldItem in e.OldItems ?? ReadOnlyCollection<T>.Empty)
                {
                    _lazy.Remove(oldItem);
                }
                _lazy.AddRange(e.NewItems ?? ReadOnlyCollection<T>.Empty);
                break;
            case NotifyCollectionChangedAction.Reset:
            {
                Reset();
                break;
            }
        }
    }
    
    private void Reset()
    {
        _lazy.Clear();
        if (_origin.List.Any())
        {
            _uiThread
                .OnUIAsync(() => _lazy.Add(_origin.List[0]))
                .FireAndForget();
        }
            
        _ = Task.Run(async () =>
        {
            for (int i = 1; i < _origin.List.Count; i++)
            {
                var index = i;
                await _lazyTick();
                _uiThread
                    .OnUIAsync(() => _lazy.Add(_origin.List[index]))
                    .FireAndForget();
            }
        });
    }
}
