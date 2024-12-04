using InventoryAPI.Services;
using StackExchange.Redis;

namespace InventoryAPI.Tests;

[TestFixture]
public class DatabaseService_SatisfiesInterface
{

    private DatabaseService _databaseService;

    [SetUp]
    public void SetUp()
    {
        _databaseService = new("Connstring");
    }
}
