using AMLWebAplication.Data;

namespace AMLWebAplication.Services
{
    public interface IMediaLoanService
    {
        Task<List<MediaLoan>> GetMediaLoansAsync();
        Task<List<MediaLoan>> GetMediaLoansByAccountIdAsync(int accountId);
        Task<List<MediaLoan>> GetOverdueLoansAsync();   
        Task<List<MediaLoan>> GetActiveLoansAsync();
    }
}