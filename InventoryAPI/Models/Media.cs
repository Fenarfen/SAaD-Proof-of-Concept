namespace InventoryAPI.Models;

public class Media
{
    public int ID { get; set; }
    public Branch Branch { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public DateTime Released { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
