using InventoryAPI.Interfaces;
using InventoryAPI.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace InventoryAPI.Repositories;

public class BranchRepository(IDatabaseService databaseService, ICacheService cacheService)
{
    private readonly IDatabaseService _databaseService = databaseService;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<List<Branch>> GetBranches(string key)
    {
        var result = await _cacheService.Get<List<Branch>>(key);
        if (result == null)
        {
            result = _databaseService.GetBranches();
            _cacheService.Set(key, result);
        }

        return result;
    }
}
