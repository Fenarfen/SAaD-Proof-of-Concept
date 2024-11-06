using InventoryAPI.Models;

namespace InventoryAPI.Interfaces;

public interface IDatabaseService
{
    public List<Media> GetAllMedia();
    public List<Media> GetMediaByCity(int cityID);
}
