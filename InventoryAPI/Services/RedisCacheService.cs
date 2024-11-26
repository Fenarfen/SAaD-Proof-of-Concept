using InventoryAPI.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace InventoryAPI.Services;

public class RedisCacheService(ConfigurationOptions config) : ICacheService
{
    private readonly ConnectionMultiplexer _redis = 
        ConnectionMultiplexer.Connect(config);

    public void Drop(string key)
    {
        _redis.GetDatabase().KeyDeleteAsync(key, CommandFlags.FireAndForget);
    }

    public async Task<T?> Get<T>(string key)
    {
        var result = await _redis.GetDatabase().StringGetAsync(key);

        if (result.IsNull)
            return default;

        return JsonSerializer.Deserialize<T>(result);
    }

    public void Set(string key, object value)
    {
        string jsonValue = JsonSerializer.Serialize(value);

        _redis.GetDatabase().StringSetAsync(
            key,
            jsonValue,
            flags: CommandFlags.FireAndForget);
    }
}
