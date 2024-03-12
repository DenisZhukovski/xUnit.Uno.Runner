using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace XUnit.Runners.Core;

public class FilteredCollection<T> : IFilteredCollection<T>, IDisposable
{
    private readonly ObservableCollection<T> _source;
    private object? _filter;

    public FilteredCollection(ObservableCollection<T> source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _source.CollectionChanged += OnCollectionChanged;
    }

    public ObservableCollection<T> List { get; } = new();

    public object? Filter
    {
        get => _filter;
        set 
        {
            if (!Equals(_filter, value))
            {
                _filter = value;
                RefreshFilter();
            }
        }
    }

    public void Dispose()
    {
        _source.CollectionChanged -= OnCollectionChanged;
        List.Clear();
    }
    
    private void RefreshFilter()
    {
        List.ReplaceWith(
            _source.Where(
                item => Equals(item ,Filter)
            )
        );
    }
    
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                Add(e.NewItems ?? ReadOnlyCollection<T>.Empty);
                break;
            case NotifyCollectionChangedAction.Remove:
                Remove(e.OldItems ?? ReadOnlyCollection<T>.Empty);
                break;
            case NotifyCollectionChangedAction.Replace:
                Remove(e.OldItems ?? ReadOnlyCollection<T>.Empty);
                Add(e.NewItems ?? ReadOnlyCollection<T>.Empty);
                break;
            case NotifyCollectionChangedAction.Reset:
            {
                RefreshFilter();
                break;
            }
        }
    }
    
    private void Add(IList itemsToAdd)
    {
        foreach (T item in itemsToAdd)
        {
            if (Equals(item, Filter))
            {
                List.Add(item);
            }
        }
    }
    
    private void Remove(IList itemsToRemove)
    {
        foreach (T item in itemsToRemove)
        {
            if (Equals(item, Filter))
            {
                List.Remove(item);
            }
        }
    }
}
