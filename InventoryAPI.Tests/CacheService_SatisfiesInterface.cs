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
            EndPoints = { "172.17.0.2:6379" },
            AbortOnConnectFail = true,
        };
        _cacheService = new(configOptions);
    }

    [Test] 
    public async Task GetKey()
    {

    }

    [Test]
    public void SetKey()
    {

    }

    [Test]
    public void DropKey()
    {

    }
}
