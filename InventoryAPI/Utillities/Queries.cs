namespace InventoryAPI.Utillities;

static public class Queries
{
    public const string GetAllMedia = @"
        SELECT 
        	m.ID, m.Title, m.Released, m.Author, m.[Type], 
        	m.BranchID AS ID, b.[Name], b.Opened, 
        	b.AddressID AS ID, a.FirstLine, a.SecondLine, a.PostCode, 
        	a.CityID AS ID, c.[Name]
        FROM AML.dbo.Media m
        INNER JOIN Branch b ON m.BranchID = b.ID
        INNER JOIN [Address] a ON b.AddressID = a.ID
        INNER JOIN City c ON a.CityID = c.ID
        LEFT JOIN ( 
        	SELECT mt.MediaID 
        	FROM MediaTransfer mt
        	WHERE mt.Completed IS NULL
        ) mt ON mt.MediaID = m.ID
        WHERE mt.MediaID IS NULL;";

    public const string GetMediaByCity = @"
        SELECT 
        	m.ID, m.Title, m.Released, m.Author, m.[Type], 
        	m.BranchID AS ID, b.[Name], b.Opened, 
        	b.AddressID AS ID, a.FirstLine, a.SecondLine, a.PostCode, 
        	a.CityID AS ID, c.[Name]
        FROM AML.dbo.Media m
        INNER JOIN Branch b ON m.BranchID = b.ID
        INNER JOIN [Address] a ON b.AddressID = a.ID
        INNER JOIN City c ON a.CityID = c.ID
        LEFT JOIN ( 
        	SELECT mt.MediaID 
        	FROM MediaTransfer mt
        	WHERE mt.Completed IS NULL
        ) mt ON mt.MediaID = m.ID
        WHERE mt.MediaID IS NULL 
        AND a.CityID = @CityID;";

    public const string GetTransferByUserID = @"
        SELECT 
        	mt.ID, mt.UserID, mt.Approved, mt.Completed, mt.Created,
        	mt.MediaID AS ID, m.Title, m.Author, m.Released, m.[Type], 
        		m.BranchID AS ID, mb.[Name], mb.Opened,
        		mb.AddressID AS ID, mba.FirstLine, mba.SecondLine, mba.PostCode, 
        		mba.CityID AS ID, mbc.[Name],
        	mt.OriginBranchID AS ID, ob.[Name], ob.Opened,
        		ob.AddressID AS ID, oba.FirstLine, oba.SecondLine, oba.PostCode, 
        		oba.CityID AS ID, obc.[Name],
        	mt.DestinationBranchID AS ID, db.[Name], db.Opened,
        		db.AddressID AS ID, dba.FirstLine, dba.SecondLine, dba.PostCode, 
        		dba.CityID AS ID, dbc.[Name]
        FROM MediaTransfer mt
        INNER JOIN Media m ON mt.MediaID = m.ID
        INNER JOIN Branch mb ON m.BranchID = mb.ID
        INNER JOIN [Address] mba ON mb.AddressID = mba.ID
        INNER JOIN City mbc ON mba.CityID = mbc.ID
        INNER JOIN Branch ob ON mt.OriginBranchID = ob.ID
        INNER JOIN [Address] oba ON ob.AddressID = oba.ID
        INNER JOIN City obc ON oba.CityID = obc.ID
        INNER JOIN Branch db ON mt.DestinationBranchID = db.ID
        INNER JOIN [Address] dba ON db.AddressID = dba.ID
        INNER JOIN City dbc ON dba.CityID = dbc.ID
        INNER JOIN [User] u ON mt.UserID = U.ID
        WHERE mt.UserID = @UserID;";

    public const string CreateTransfer = @"
        INSERT INTO MediaTransfer (MediaID, OriginBranchID, DestinationBranchID, UserID)
        OUTPUT inserted.ID
        VALUES (@MediaID,@OriginBranchID,@DestinationBranchID,@UserID);";

    public const string GetMediaSplitOn = "ID,ID,ID";

    public const string GetMediaTransferSplitOn = "ID,ID,ID,ID,ID,ID,ID,ID,ID,ID";
}
