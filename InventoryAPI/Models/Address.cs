namespace InventoryAPI.Models;

public class Address
{
    public int ID { get; set; }
    public City City { get; set; } = new();
    public string FirstLine { get; set; } = string.Empty;
    public string SecondLine { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;   
}
