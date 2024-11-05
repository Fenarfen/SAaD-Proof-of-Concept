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
                splitOn: Utillities.Queries.GetAllMediaSplitOn)
                .ToList();
        }

        return results;
    }
}
