using Dapper;
using InventoryAPI.Interfaces;
using InventoryAPI.Models;
using Microsoft.Data.SqlClient;

namespace InventoryAPI.Services;

public class DatabaseService : IDatabaseService
{
    private readonly string _connString;

    public DatabaseService(string connString)
    {
        _connString = connString;
    }

    public List<Media> GetAllMedia()
    {
        List<Media> results = [];

        using(var connection = new SqlConnection(_connString))
        {
            connection.Open();

            results = connection.Query<Media, Branch, Address, City, Media>(
                Utillities.Queries.GetAllMedia,
                (media, branch, address, city) =>
                {
                    address.City = city;
                    branch.Address = address;
                    media.Branch = branch;
                    return media;
                },
                splitOn: Utillities.Queries.GetMediaSplitOn)
                .ToList();
        }

        return results;
    }

    public List<Media> GetMediaByCity(int cityID)
    {
        List<Media> results = [];

        // Create object with parameter for query
        var parameter = new { CityID = cityID };

        using (var connection = new SqlConnection(_connString))
        {
            connection.Open();

            results = connection.Query<Media, Branch, Address, City, Media>(
                Utillities.Queries.GetMediaByCity,
                (media, branch, address, city) =>
                {
                    address.City = city;
                    branch.Address = address;
                    media.Branch = branch;
                    return media;
                },
                splitOn: Utillities.Queries.GetMediaSplitOn,
                param: parameter)
                .ToList();
        }

        return results;
    }

    public List<MediaTransfer> GetTransfers(int userID)
    {
        var parameter = new { UserID = userID};

        List<MediaTransfer> results = [];

        using (var connection = new SqlConnection(_connString))
        {
            connection.Open();

            Type[] returnTypes = 
            {
                typeof(MediaTransfer),
                typeof(Media),
                typeof(Branch),
                typeof(Address),
                typeof(City),
                typeof(Branch),
                typeof(Address),
                typeof(City),
                typeof(Branch),
                typeof(Address),
                typeof(City),
            };

            results = connection.Query<MediaTransfer>(
                Utillities.Queries.GetTransferByUserID,
                returnTypes,
                (objects) =>
                {
                    var mediaTransfer = (MediaTransfer)objects[0];
                    var media = (Media)objects[1];
                    media.Branch = (Branch)objects[2];
                    media.Branch.Address = (Address)objects[3];
                    media.Branch.Address.City = (City)objects[4];
                    mediaTransfer.Media = media;
                    var originBranch = (Branch)objects[5];
                    originBranch.Address = (Address)objects[6];
                    originBranch.Address.City = (City)objects[7];
                    mediaTransfer.OriginBranch = originBranch;
                    var destinationBranch = (Branch)objects[8];
                    destinationBranch.Address = (Address)objects[9];
                    destinationBranch.Address.City = (City)objects[10];
                    mediaTransfer.DestinationBranch = destinationBranch;
                    return mediaTransfer;
                },
                splitOn: Utillities.Queries.GetMediaTransferSplitOn,
                param: parameter)
                .ToList();
        }

        return results;
    }

    public int CreateMediaTransfer(MediaTransfer mediaTransfer)
    {
        var parameters = new  
        {
            MediaID = mediaTransfer.Media.ID,
            OriginBranchID = mediaTransfer.OriginBranch.ID,
            DestinationBranchID = mediaTransfer.DestinationBranch.ID,
            UserID = mediaTransfer.UserID,
        };

        using (var connection = new SqlConnection(_connString))
        {
            connection.Open();

             return connection.QuerySingle<int>(
                Utillities.Queries.CreateTransfer,
                param: parameters);
        }
    }
}
