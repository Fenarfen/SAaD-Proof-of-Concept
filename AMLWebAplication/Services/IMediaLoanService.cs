using AMLWebAplication.Data;

namespace AMLWebAplication.Services
{
    public interface IMediaLoanService
    {
        Task<List<MediaLoan>> GetMediaLoansAsync();
        Task<List<MediaLoan>> GetLoansByAccountAsync(int accountId);
        Task<List<MediaLoan>> GetOverdueLoansAsync();   
        Task<List<MediaLoan>> GetActiveLoansAsync();
        Task<BranchReport> GetBranchReportAsync(int branchId, DateTime startDate, DateTime endDate);
    }
}