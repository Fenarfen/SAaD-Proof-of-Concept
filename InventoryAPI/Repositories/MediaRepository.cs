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

    public List<Media> GetMediaByCity(string city)
    {
        return _dbService.GetMediaByCity(city);
    }

    public List<MediaTransfer> GetTransfers(int userID)
    {
        return _dbService.GetTransfers(userID);
    }

    public int CreateTransfer(MediaTransfer mediaTransfer)
    {
        return _dbService.CreateMediaTransfer(mediaTransfer);
    }
}
