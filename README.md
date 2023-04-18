
## Usage

You can create a new instance of `InMemoryCache` by specifying the maximum number of items it can hold in the constructor. For example:

     var cache = new InMemoryCache<string, int>(capacity: 10);

You can then add or update items in the cache using the `AddOrUpdate` method. If the Key is already present update is performed, otherwise it will be added

    cache.AddOrUpdate("key1", 1);

You can retrieve items from the cache using the `TryGetValue` method, which returns `true` if the key is found and `false` otherwise:

    if (cache.TryGetValue("key1", out var value))
    {
        Console.WriteLine($"The value of key1 is {value}.");
    } 

You can remove items from the cache using the `Remove` method:

    cache.Remove("key1");

You can clear the cache using the `Clear` method:

    cache.Clear(); 

The `OnItemEvicted` event is raised when an item is automatically removed from the cache:

    cache.OnItemEvicted += (sender, args) => {
        Console.WriteLine($"The item with key {args.Key} and value {args.Value} was evicted from the cache.");
    }; 

