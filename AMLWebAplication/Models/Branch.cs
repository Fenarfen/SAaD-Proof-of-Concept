namespace AMLWebAplication.Models;

public class Branch
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AddressFirstLine { get; set; } = string.Empty;
    public string AddressSecondLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;
    public DateTime Opened { get; set; }
}
