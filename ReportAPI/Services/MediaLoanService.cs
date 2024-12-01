using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ReportAPI.Models;
using ReportAPI.DTOs;
using ReportAPI.Services;

namespace LibraryReportingSystem.API.Services
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
                        WHEN DueDate < GETDATE() AND Status = 'Active' THEN 'Overdue'
                        ELSE Status
                    END as Status
                FROM MediaLoan
                WHERE AccountID = @AccountId
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

        public async Task<BranchReportDto> GetBranchReportAsync(int branchId, DateTime startDate, DateTime endDate)
        {
            var report = new BranchReportDto
            {
                BranchId = branchId,
                StartDate = startDate,
                EndDate = endDate
            };

            // Borrowing stats query
            const string borrowingStatsSql = @"
                SELECT 
                    COUNT(*) as TotalLoans,
                    COUNT(DISTINCT AccountID) as UniqueUsers,
                    COUNT(CASE WHEN Status = 'Overdue' THEN 1 END) as OverdueLoans
                FROM MediaLoan
                WHERE BranchID = @BranchId
                AND LoanedDate BETWEEN @StartDate AND @EndDate";

            report.BorrowingStats = await _db.QuerySingleAsync<BorrowingStatsDto>(
                borrowingStatsSql,
                new { BranchId = branchId, StartDate = startDate, EndDate = endDate }
            );

            // Popular media query
            const string popularMediaSql = @"
                SELECT TOP 10
                    m.MediaID,
                    m.Title,
                    COUNT(*) as LoanCount
                FROM MediaLoan ml
                JOIN Media m ON ml.MediaID = m.MediaID
                WHERE ml.BranchID = @BranchId
                AND ml.LoanedDate BETWEEN @StartDate AND @EndDate
                GROUP BY m.MediaID, m.Title
                ORDER BY LoanCount DESC";

            report.PopularItems = await _db.QueryAsync<PopularMediaItemDto>(
                popularMediaSql,
                new { BranchId = branchId, StartDate = startDate, EndDate = endDate }
            );

            return report;
        }
        public async Task<List<MediaLoan>> GetLoansByBranchAsync(int branchId, DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                SELECT MediaID, AccountID, BranchID, LoanedDate, DueDate, ReturnedDate, Status
                FROM MediaLoan
                WHERE BranchID = @BranchId
                AND LoanedDate BETWEEN @StartDate AND @EndDate";

            return (await _db.QueryAsync<MediaLoan>(sql, new { BranchId = branchId, StartDate = startDate, EndDate = endDate })).ToList();
        }

        public async Task<List<PopularMediaItem>> GetPopularMediaItemsAsync(int branchId, DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                SELECT TOP 10
                    m.MediaID,
                    m.Title,
                    COUNT(*) as LoanCount
                FROM MediaLoan ml
                JOIN Media m ON ml.MediaID = m.MediaID
                WHERE ml.BranchID = @BranchId
                AND ml.LoanedDate BETWEEN @StartDate AND @EndDate
                GROUP BY m.MediaID, m.Title
                ORDER BY LoanCount DESC";

            return (await _db.QueryAsync<PopularMediaItem>(sql, new { BranchId = branchId, StartDate = startDate, EndDate = endDate })).ToList();
        }
    }
}
