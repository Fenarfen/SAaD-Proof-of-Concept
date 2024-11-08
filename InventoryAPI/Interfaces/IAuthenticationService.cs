using InventoryAPI.Models;

namespace InventoryAPI.Interfaces;

public interface IAuthenticationService
{
    public string GetToken(HttpRequest request);
    public Task<Account?> GetAccount(string token);
}
