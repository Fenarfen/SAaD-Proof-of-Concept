﻿namespace InventoryAPI.Models;

public class MediaTransfer
{
    public int ID { get; set; }
    public Media Media { get; set; } = new();
    public Branch OriginBranch { get; set; } = new();
    public Branch DestinationBranch { get; set; } = new();
    public int AccountID { get; set; }
    public DateTime? Approved { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Completed { get; set; }
}
