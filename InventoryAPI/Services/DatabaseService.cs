using Dapper;
using InventoryAPI.Interfaces;
using InventoryAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

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

            results = connection.Query<Media, Branch, Media>(
                Utillities.Queries.GetAllMedia,
                (media, branch) =>
                {
                    media.Branch = branch;
                    return media;
                },
                splitOn: Utillities.Queries.GetMediaSplitOn)
                .ToList();
        }

        return results;
    }

    public List<Media> GetMediaByCity(string city)
    {
        List<Media> results = [];

        // Create object with parameter for query
        var parameter = new { City = city };

        using (var connection = new SqlConnection(_connString))
        {
            connection.Open();

            results = connection.Query<Media, Branch, Media>(
                Utillities.Queries.GetMediaByCity,
                (media, branch) =>
                {
                    media.Branch = branch;
                    return media;
                },
                splitOn: Utillities.Queries.GetMediaSplitOn,
                param: parameter)
                .ToList();
        }

        return results;
    }

    public List<MediaTransfer> GetTransfers(int accountID)
    {
        var parameter = new { AccountID = accountID };

        List<MediaTransfer> results = [];

        using (var connection = new SqlConnection(_connString))
        {
            connection.Open();

            results = connection.Query<MediaTransfer, Media, Branch, Branch, Branch, MediaTransfer>(
                Utillities.Queries.GetTransferByAccountID,
                (mediaTransfer, media, mediaBranch, originBranch, destinationBranch) =>
                {
                    media.Branch = mediaBranch;
                    mediaTransfer.Media = media;
                    mediaTransfer.OriginBranch = originBranch;
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
            AccountID = mediaTransfer.AccountID,
        };

        using (var connection = new SqlConnection(_connString))
        {
            connection.Open();

             return connection.QuerySingle<int>(
                Utillities.Queries.CreateTransfer,
                param: parameters);
        }
    }

    public List<Branch> GetBranches()
    {
        using (var connection = new SqlConnection(_connString))
        {
            connection.Open();

            return connection.Query<Branch>(
                Utillities.Queries.GetBranches)
                .ToList();
        }
    }
}
