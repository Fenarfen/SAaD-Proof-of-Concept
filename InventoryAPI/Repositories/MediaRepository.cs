using InventoryAPI.Models;
using InventoryAPI.Services;
using Microsoft.AspNetCore.SignalR;

namespace InventoryAPI.Repositories;

public class MediaRepository(DatabaseService databaseService)
{
    private readonly DatabaseService _dbService = databaseService;

    public List<Media> GetAllMedia()
    {
        string query;

        // send to _dbService

        return [];
    }
}
