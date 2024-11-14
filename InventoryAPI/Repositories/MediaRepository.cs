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
