namespace CacheComponent;

public class ItemEvictedEventArgs<TKey, TValue> : EventArgs
{
    public TKey Key { get; }
    public TValue Value { get; }

    public ItemEvictedEventArgs(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}