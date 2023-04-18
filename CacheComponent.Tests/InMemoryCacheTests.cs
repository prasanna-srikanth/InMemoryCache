namespace CacheComponent.Tests;

public class InMemoryCacheTests
{
    [Theory]
    [InlineData("key1", 1)]
    [InlineData("key2", "string value")]
    [InlineData("key3", true)]
    [MemberData(nameof(GetAnonymousObjects))]
    public void GetCacheItem_ReturnsCorrectValue_ForAddedItem<TKey, TValue>(TKey key, TValue? value) where TKey : notnull
    {
        // Arrange
        var cache = new InMemoryCache<TKey, TValue?>(1);

        // Act
        cache.AddOrUpdate(key, value);
        var result = cache.TryGetValue(key, out var retrievedValue);

        // Assert
        result.Should().BeTrue();
        retrievedValue.Should().Be(value);
    }

    [Theory]
    [InlineData("key", 1)]
    public void GetCacheItem_ReturnsLatestValue_ForUpdatedItem<TKey, TValue>(TKey key, TValue? value) where TKey : notnull
    {
        // Arrange
        var cache = new InMemoryCache<TKey, TValue?>(1);
        cache.AddOrUpdate(key, value);

        // Act
        var updatedValue = (dynamic) 2;
        cache.AddOrUpdate(key, updatedValue);

        var result = cache.TryGetValue(key, out var retrievedValue);

        // Assert
        result.Should().BeTrue();
        retrievedValue.Should().Be(updatedValue);
    }

    [Theory]
    [InlineData("key", 1)]
    public void GetCacheItem_ReturnsFalse_IfKeyIsNotPresent<TKey, TValue>(TKey key, TValue? value) where TKey : notnull
    {
        // Arrange
        var cache = new InMemoryCache<TKey, TValue?>(1);

        // Act
        var result = cache.TryGetValue(key, out var retrievedValue);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("first key", 1)]
    [InlineData("second key", "string value")]
    [InlineData("third key", true)]
    public void GetCacheItem_ReturnsFalse_WhenCacheIsCleared<TKey, TValue>(TKey key, TValue? value) where TKey : notnull
    {
        // Arrange
        var cache = new InMemoryCache<TKey, TValue?>(1);

        // Act
        cache.AddOrUpdate(key, value);

        cache.Clear();
        var result = cache.TryGetValue(key, out _);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("first key", 1)]
    [InlineData("second key", "string value")]
    [InlineData("third key", true)]
    public void GetCacheItem_ReturnsFalse_WhenKeyIsRemovedFromCache<TKey, TValue>(TKey key1, TValue? value1) where TKey : notnull
    {
        // Arrange
        var cache = new InMemoryCache<TKey, TValue?>(1);

        // Act
        cache.AddOrUpdate(key1, value1);
        var hasRemoved = cache.Remove(key1);
        var result = cache.TryGetValue(key1, out _);

        // Assert
        hasRemoved.Should().BeTrue();
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("key1", 1, "key2", 2, "key3", 3)]
    public void CanEvictRecentlyUsedItem<TKey, TValue>(TKey key1, TValue? value1, TKey key2, TValue? value2, TKey key3, TValue? value3) where TKey : notnull
    {
        // Arrange
        var cache = new InMemoryCache<TKey, TValue?>(2);

        // Act
        cache.AddOrUpdate(key1, value1);
        cache.AddOrUpdate(key2, value2);
        cache.AddOrUpdate(key3, value3);

        var result1 = cache.TryGetValue(key1, out _);
        var result2 = cache.TryGetValue(key2, out var retrievedValue2);
        var result3 = cache.TryGetValue(key3, out var retrievedValue3);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeTrue();
        result3.Should().BeTrue();
        retrievedValue2.Should().Be(value2);
        retrievedValue3.Should().Be(value3);
    }

    [Fact]
    public void AddOrUpdate_RaiseEvent_WhenAnItemIsEvicted()
    {
        // Arrange
        var cache = new InMemoryCache<string, int>(2);
        var evictedEventArgs = default(ItemEvictedEventArgs<string, int>);
        cache.OnItemEvicted += (sender, args) => evictedEventArgs = args;

        // Act
        cache.AddOrUpdate("key1", 1);
        cache.AddOrUpdate("key2", 2);
        cache.AddOrUpdate("key3", 3);

        // Assert
        evictedEventArgs.Should().NotBeNull();
        evictedEventArgs.Key.Should().Be("key1");
        evictedEventArgs.Value.Should().Be(1);
    }


    public static IEnumerable<object[]> GetAnonymousObjects()
    {
        yield return new object[]
        {
            new { Id = "1", Title = "Hello" },
            new { Id = "2", Title = "World" }
        };
    }
}
