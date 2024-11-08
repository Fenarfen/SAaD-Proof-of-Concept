namespace InventoryAPI.Models;

public class User
{
    public int ID { get; set; }
    public Token Token { get; set; } = new();
    public Role Role { get; set; } = new();
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public bool Verified { get; set; } 
}
