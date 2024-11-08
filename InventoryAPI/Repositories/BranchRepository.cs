using InventoryAPI.Interfaces;
using InventoryAPI.Models;

namespace InventoryAPI.Repositories;

public class BranchRepository(IDatabaseService databaseService)
{
    private readonly IDatabaseService _databaseService = databaseService;

    public List<Branch> GetBranches()
    {
        return _databaseService.GetBranches();
    }
}
