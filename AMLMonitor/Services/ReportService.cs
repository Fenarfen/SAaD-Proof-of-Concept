using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using AMLMonitor.Models.Reports;
using AMLMonitor.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AMLMonitor.Services
{
    public class ReportService : IReportService
    {
        private readonly string _connectionString;

        public ReportService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<MediaUsageReport>> GetMediaUsageReport(int branchId, DateTime startDate, DateTime endDate)
        {
            const string query = @"
                SELECT 
                    m.Title AS MediaTitle,
                    m.Type AS MediaType,
                    COUNT(ml.ID) AS BorrowedCount
                FROM MediaLoan ml
                INNER JOIN Media m ON ml.MediaID = m.ID
                WHERE ml.BranchID = @BranchID
                  AND ml.LoanedDate BETWEEN @StartDate AND @EndDate
                GROUP BY m.Title, m.Type
                ORDER BY BorrowedCount DESC";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<MediaUsageReport>(query, new
                {
                    BranchID = branchId,
                    StartDate = startDate,
                    EndDate = endDate
                });
            }
        }
    }

}