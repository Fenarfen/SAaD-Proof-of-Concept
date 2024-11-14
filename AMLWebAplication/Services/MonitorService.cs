using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using AMLWebAplication.Data;
using Microsoft.Extensions.Configuration;

namespace AMLWebAplication.Services
{
    public class MonitorService : IMonitorService
    {
        private readonly IDbConnection _db;

        public MonitorService(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            const string sql = "SELECT ID, Username, FirstName, LastName, Email, Created FROM Account";
            return (await _db.QueryAsync<Account>(sql)).ToList();
        }
    }
}