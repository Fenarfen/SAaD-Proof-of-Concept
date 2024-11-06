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
        AND a.CityID = 1;";

    public const string GetMediaSplitOn = "ID,ID,ID";
}
