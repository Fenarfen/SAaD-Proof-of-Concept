namespace InventoryAPI.Models;

public class Branch
{
    public int ID { get; set; }
    public Address Address { get; set; } = new Address();
    public string Name { get; set; } = string.Empty;
    public DateTime Opened { get; set; }
}
