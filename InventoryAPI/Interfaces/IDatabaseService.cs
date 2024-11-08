using InventoryAPI.Models;

namespace InventoryAPI.Interfaces;

public interface IDatabaseService
{
    public List<Media> GetAllMedia();
    public List<Media> GetMediaByCity(string city);
    public List<MediaTransfer> GetTransfers(int userID);
    public int CreateMediaTransfer(MediaTransfer mediaTransfer);
}
