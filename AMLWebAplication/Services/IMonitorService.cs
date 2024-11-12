using AMLWebAplication.Data;

namespace AMLWebAplication.Services
{
    public interface IMonitorService
    {
        Task<List<Account>> GetAccountsAsync();
    }
}