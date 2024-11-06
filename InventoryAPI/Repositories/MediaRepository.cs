using InventoryAPI.Interfaces;
using InventoryAPI.Models;

namespace InventoryAPI.Repositories;

public class MediaRepository(IDatabaseService databaseService)
{
    private readonly IDatabaseService _dbService = databaseService;

    public List<Media> GetAllMedia()
    {
        return _dbService.GetAllMedia();
    }

    public List<Media> GetMediaByCity(int cityID)
    {
        return _dbService.GetMediaByCity(cityID);
    }
}
