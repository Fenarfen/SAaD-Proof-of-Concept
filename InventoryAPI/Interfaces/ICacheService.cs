namespace InventoryAPI.Interfaces;

public interface ICacheService
{
    public Task<T?> Get<T>(string key);
    public void Set(string key, object value);
    public void Drop(string key);
}
