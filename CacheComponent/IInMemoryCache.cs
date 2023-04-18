namespace CacheComponent;

public interface IInMemoryCache<TKey, TValue> where TKey : notnull
{
    void AddOrUpdate(TKey key, TValue? value);

    bool TryGetValue(TKey key, out TValue? value);

    bool Remove(TKey key);

    void Clear();

    event EventHandler<ItemEvictedEventArgs<TKey, TValue>>? OnItemEvicted;
}