using InventoryAPI.Models;

namespace InventoryAPI.Interfaces;

public interface IDatabaseService
{
    public List<Media> GetAllMedia();
    public List<Media> GetMediaByCity(string city);
    public List<MediaTransfer> GetTransfers(int accountID);
    public int CreateMediaTransfer(MediaTransfer mediaTransfer);

    public List<Branch> GetBranches();
}
