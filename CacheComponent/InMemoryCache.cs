using System.Collections.Concurrent;

namespace CacheComponent;

public class InMemoryCache<TKey, TValue> : IInMemoryCache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly ConcurrentDictionary<TKey, TValue> _dictionary;
    private readonly ConcurrentQueue<TKey> _queue;

    public InMemoryCache(int capacity)
    {
        _capacity = capacity;
        _dictionary = new ConcurrentDictionary<TKey, TValue>();
        _queue = new ConcurrentQueue<TKey>();
    }

    public void AddOrUpdate(TKey key, TValue value)
    {
        _dictionary.AddOrUpdate(key, value, (_, _) => value);

        if (!_queue.Contains(key))
            _queue.Enqueue(key);

        if (_queue.Count > _capacity)
        {
            if (_queue.TryDequeue(out var removedKey))
            {
                if (_dictionary.TryRemove(removedKey, out var removedValue))
                    OnItemEvicted?.Invoke(this, new ItemEvictedEventArgs<TKey, TValue>(removedKey, removedValue));
            }
        }
    }

    public bool TryGetValue(TKey key, out TValue? value)
    {
        if (_dictionary.TryGetValue(key, out value))
        {
            _queue.Enqueue(key);
            return true;
        }

        return false;
    }

    public bool Remove(TKey key)
    {
        return _dictionary.TryRemove(key, out _);
    }

    public void Clear()
    {
        _dictionary.Clear();
        _queue.Clear();
    }

    public event EventHandler<ItemEvictedEventArgs<TKey, TValue>>? OnItemEvicted;

    protected void RaiseOnItemEvicted(ItemEvictedEventArgs<TKey, TValue> args)
    {
        OnItemEvicted?.Invoke(this, args);
    }
}