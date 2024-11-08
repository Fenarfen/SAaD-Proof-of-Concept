namespace InventoryAPI.Models;

public class MemberAddress
{
    public int Id { get; set; }
    public string FirstLine { get; set; } = string.Empty;
    public string SecondLine { get; set; } = string.Empty;
    public string ThirdLine {  get; set; } = string.Empty;
    public string FourthLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty; 
    public string PostCode { get; set; } = string.Empty;
}
