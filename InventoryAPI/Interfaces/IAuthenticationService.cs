using InventoryAPI.Models;

namespace InventoryAPI.Interfaces;

public interface IAuthenticationService
{
    public string GetToken(HttpRequest request);
    public Task<User?> GetUser(string token);
}
