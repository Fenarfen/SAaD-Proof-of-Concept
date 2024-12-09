using InventoryAPI.Services;

namespace InventoryAPI.Tests;

[TestFixture]
public class DatabaseService_SatisfiesInterface
{
    private DatabaseService _databaseService;

    [SetUp]
    public void SetUp()
    {
        _databaseService = new("Data Source=localhost\\SQLEXPRESS,55921;Database=AML;User Id=inventoryAPI;Password=1nventoryAP!;TrustServerCertificate=True;");
    }

    [Test]
    public void GetMedia()
    {
        var results = _databaseService.GetAllMedia();

        Assert.Multiple(() =>
        {
            Assert.That(results, Is.Not.Null);
            Assert.That(results, Is.Not.Empty);
        });
    }

    [Test]
    public void GetBranches()
    {
        var results = _databaseService.GetBranches();

        Assert.Multiple(() =>
        {
            Assert.That(results, Is.Not.Null);
            Assert.That(results, Is.Not.Empty);
        });
    }

    [Test]
    public void GetTransfers()
    {
        var results = _databaseService.GetTransfers(1);

        Assert.Multiple(() =>
        {
            Assert.That(results, Is.Not.Null);
            Assert.That(results, Is.Not.Empty);
        });
    }

    [Test]
    public void GetMediaByCity()
    {
        var results = _databaseService.GetMediaByCity("Sheffield");

        Assert.Multiple(() =>
        {
            Assert.That(results, Is.Not.Null);
            Assert.That(results, Is.Not.Empty);
        });
    }
}
