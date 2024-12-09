using InventoryAPI.Services;
using StackExchange.Redis;

namespace InventoryAPI.Tests;

[TestFixture]
public class CacheService_SatisfiesInterface
{
    private RedisCacheService _cacheService;

    [SetUp]
    public void SetUp()
    {
        ConfigurationOptions configOptions = new()
        {
            EndPoints = { "localhost:6379" },
        };
        _cacheService = new(configOptions);
    }

    [Test, Order(1)]
    public async Task SetKey()
    {
        _cacheService.Set("TEST", "testString");

        var result = await _cacheService.Get<string>("TEST");

        Assert.That(result, Is.EqualTo("testString"));
    }

    [Test, Order(2)] 
    public async Task GetKey()
    {
        var result = await _cacheService.Get<string>("TEST");

        Assert.That(result, Is.Not.Null);
    }

    [Test, Order(3)]
    public async Task DropKey()
    {
        _cacheService.Drop("TEST");

        var result = await _cacheService.Get<string>("TEST");

        Assert.That(result, Is.Null);
    }
}
