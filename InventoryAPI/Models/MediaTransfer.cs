namespace InventoryAPI.Models;

public class MediaTransfer
{
    public int ID { get; set; }
    public Media Media { get; set; } = new();
    public Branch OriginBranch { get; set; } = new();
    public Branch DestinationBranch { get; set; } = new();
    public int UserID { get; set; }
    public bool? Approved { get; set; }
    public DateTime Created { get; set; }
    public bool? Completed { get; set; }
}
