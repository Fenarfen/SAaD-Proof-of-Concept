using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using AMLWebAplication.Data;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace AMLWebAplication.Services
{
    public class MediaLoanService : IMediaLoanService
    {
        private readonly IDbConnection _db;

        public MediaLoanService(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<MediaLoan>> GetMediaLoansAsync()
        {
            const string sql = "SELECT MediaID, AccountID, BranchID, LoanedDate, DueDate, ReturnedDate, Status FROM MediaLoan";
            return (await _db.QueryAsync<MediaLoan>(sql)).ToList();
        }

        public async Task<List<MediaLoan>> GetLoansByAccountAsync(int accountId)
        {
            const string sql = @"
                SELECT MediaID, AccountID, BranchID, LoanedDate, DueDate,
                    CASE
                        WHEN DueDate < GETDATE() THEN 'Overdue'
                        ELSE Status
                    END as Status
                FROM MediaLoan
                WHERE AccountID = @accountId
                AND (Status = 'Active' OR Status = 'Overdue')
                ORDER BY LoanedDate DESC";

            return (await _db.QueryAsync<MediaLoan>(sql, new { AccountId = accountId })).ToList();
        }

        public async Task<List<MediaLoan>> GetOverdueLoansAsync()
        {
            const string sql = "SELECT MediaID, AccountID, BranchID, LoanedDate, DueDate, ReturnedDate, Status FROM MediaLoan WHERE DueDate < GETDATE() AND ReturnedDate IS NULL";
            return (await _db.QueryAsync<MediaLoan>(sql)).ToList();
        }

        public async Task<List<MediaLoan>> GetActiveLoansAsync()
        {
            const string sql = "SELECT MediaID, AccountID, BranchID, LoanedDate, DueDate, ReturnedDate, Status FROM MediaLoan WHERE ReturnedDate IS NULL";
            return (await _db.QueryAsync<MediaLoan>(sql)).ToList();
        }
    }

}