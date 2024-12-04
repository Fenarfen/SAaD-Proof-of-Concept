namespace InventoryAPI.Utillities;

static public class Queries
{
    public const string GetAllMedia = @"
        SELECT 
           	m.ID, m.Title, m.Released, m.Genre, m.Author, m.[Type], 
           	m.BranchID AS ID, b.[Name], b.Opened, b.AddressFirstLine, b.AddressSecondLine, b.City, b.PostCode
        FROM AML.dbo.Media m
        INNER JOIN Branch b ON m.BranchID = b.ID
        LEFT JOIN ( 
           	SELECT mt.MediaID 
           	FROM MediaTransfer mt
           	WHERE mt.Completed IS NULL
        ) mt ON mt.MediaID = m.ID
        WHERE mt.MediaID IS NULL;";

    public const string GetMediaByCity = @"
        SELECT 
           	m.ID, m.Title, m.Released, m.Genre, m.Author, m.[Type], 
           	m.BranchID AS ID, b.[Name], b.Opened, b.AddressFirstLine, b.AddressSecondLine, b.City, b.PostCode
        FROM AML.dbo.Media m
        INNER JOIN Branch b ON m.BranchID = b.ID
        LEFT JOIN ( 
           	SELECT mt.MediaID 
           	FROM MediaTransfer mt
           	WHERE mt.Completed IS NULL
        ) mt ON mt.MediaID = m.ID
        WHERE mt.MediaID IS NULL 
        AND b.City = @City;";

    public const string GetTransferByAccountID = @"
        SELECT 
           	mt.ID, mt.AccountID, mt.Approved, mt.Completed, mt.Created,
           	mt.MediaID AS ID, m.Title, m.Author, m.Genre, m.Released, m.[Type], 
            m.BranchID AS ID, mb.[Name], mb.Opened, mb.AddressFirstLine, mb.AddressSecondLine, mb.City, mb.PostCode,
           	mt.OriginBranchID AS ID, ob.[Name], ob.Opened, ob.AddressFirstLine, ob.AddressSecondLine, ob.City, ob.PostCode,
           	mt.DestinationBranchID AS ID, db.[Name], db.Opened, db.AddressFirstLine, db.AddressSecondLine, db.City, db.PostCode
        FROM MediaTransfer mt
        INNER JOIN Media m ON mt.MediaID = m.ID
        INNER JOIN Branch mb ON m.BranchID = mb.ID
        INNER JOIN Branch ob ON mt.OriginBranchID = ob.ID
        INNER JOIN Branch db ON mt.DestinationBranchID = db.ID
        WHERE mt.AccountID = @AccountID;";

    public const string CreateTransfer = @"
        INSERT INTO MediaTransfer (MediaID, OriginBranchID, DestinationBranchID, AccountID)
        OUTPUT inserted.ID
        VALUES (@MediaID,@OriginBranchID,@DestinationBranchID,@AccountID);";

    public const string GetBranches = @"
        SELECT [ID]
            ,[Name]
            ,[AddressFirstLine]
            ,[AddressSecondLine]
            ,[City]
            ,[PostCode]
            ,[Opened]
        FROM [dbo].[Branch]";

    public const string GetMediaSplitOn = "ID,ID";

    public const string GetMediaTransferSplitOn = "ID,ID,ID,ID";
}
