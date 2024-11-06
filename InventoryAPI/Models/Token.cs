namespace InventoryAPI.Models;

public class Token
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public string Value { get; set; } = string.Empty;
    public DateTime Created { get; set; }
}
