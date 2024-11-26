using InventoryAPI.Interfaces;
using InventoryAPI.Models;

namespace InventoryAPI.Repositories;

public class MediaRepository(IDatabaseService databaseService, ICacheService cacheService)
{
    private readonly IDatabaseService _dbService = databaseService;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<List<Media>> GetAllMedia(string key)
    {
        var result = await _cacheService.Get<List<Media>>(key);
        if (result == null)
        {
            result = _dbService.GetAllMedia();
            _cacheService.Set(key, result);
        }
        return result;
    }

    public async Task<List<Media>> GetMediaByCity(string key, string city)
    {
        key += "/" + city;
        var result = await _cacheService.Get<List<Media>>(key);
        if (result == null)
        {
            result = _dbService.GetMediaByCity(city);
            _cacheService.Set(key, result);
        }
        return result;
    }

    public async Task<List<MediaTransfer>> GetTransfers(string key, int userID)
    {
        key += "/" + userID;
        var result = await _cacheService.Get<List<MediaTransfer>>(key);
        if (result == null)
        {
            result = _dbService.GetTransfers(userID);
            _cacheService.Set(key, result);
        }
        return result;
    }

    public int CreateTransfer(string key, MediaTransfer mediaTransfer)
    {
        string baseKey = key.Replace("/Transfer", "");

        var result = _dbService.CreateMediaTransfer(mediaTransfer);
        
        // Drop related cache keys
        _cacheService.Drop("/api/Media");
        _cacheService.Drop(baseKey);
        _cacheService.Drop(baseKey + "/" + mediaTransfer.OriginBranch.City);
        _cacheService.Drop(key + "/" + mediaTransfer.AccountID);
        
        return result;
    }
}
