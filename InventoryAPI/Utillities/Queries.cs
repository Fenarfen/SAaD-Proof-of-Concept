namespace InventoryAPI.Utillities;

static public class Queries
{
    public const string GetAllMedia = @"
        SELECT m.ID, m.Title, m.Released, m.Author, m.[Type], 
	    m.BranchID AS ID, b.[Name], b.Opened, 
        b.AddressID AS ID, a.FirstLine, a.SecondLine, a.PostCode, 
        a.CityID AS ID, c.[Name]
        FROM AML.dbo.Media m
        INNER JOIN Branch b ON m.BranchID = b.ID
        INNER JOIN [Address] a ON b.AddressID = a.ID
        INNER JOIN City c ON a.CityID = c.ID";

    public const string GetMediaByCity = @"
        SELECT m.ID, m.Title, m.Released, m.Author, m.[Type], 
        m.BranchID AS ID, b.[Name], b.Opened, 
        b.AddressID AS ID, a.FirstLine, a.SecondLine, a.PostCode, 
        a.CityID AS ID, c.[Name]
        FROM AML.dbo.Media m
        INNER JOIN Branch b ON m.BranchID = b.ID
        INNER JOIN [Address] a ON b.AddressID = a.ID
        INNER JOIN City c ON a.CityID = c.ID
        WHERE a.CityID = @CityID";

    public const string GetMediaSplitOn = "ID,ID,ID";
}
