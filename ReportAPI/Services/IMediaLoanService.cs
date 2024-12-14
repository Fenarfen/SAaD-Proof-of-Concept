using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReportAPI.Models;
using ReportAPI.DTOs;

namespace ReportAPI.Services
{
    public interface IMediaLoanService
    {
        Task<List<MediaLoan>> GetMediaLoansAsync();
        Task<List<MediaLoan>> GetLoansByAccountAsync(int accountId);
        Task<List<MediaLoan>> GetOverdueLoansAsync();
        Task<List<MediaLoan>> GetActiveLoansAsync();
        Task<BranchReportDto> GetBranchReportAsync(int branchId, DateTime startDate, DateTime endDate);
        Task<List<MediaLoan>> GetLoansByBranchAsync(int branchId, DateTime startDate, DateTime endDate);
        Task<List<PopularMediaItem>> GetPopularMediaItemsAsync(int branchId, DateTime startDate, DateTime endDate);
    }
}