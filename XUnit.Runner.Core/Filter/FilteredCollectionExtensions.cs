namespace XUnit.Runners.Core;

public static class FilteredCollectionExtensions
{
    public static IFilteredCollection<T> Delayed<T>(this IFilteredCollection<T> collection)
    {
        return collection.Delayed(TimeSpan.FromMilliseconds(500));
    }
    
    public static IFilteredCollection<T> Delayed<T>(this IFilteredCollection<T> collection, TimeSpan delay)
    {
        return new DelayedFilteredCollection<T>(collection, delay);
    }
}
