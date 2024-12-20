using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReportAPI.Services;
using ReportAPI.DTOs;
using ReportAPI.Models;

// endpoint return ex: /api/report/branch/1/report?startDate=2023-01-01&endDate=2024-12-31
namespace ReportAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediaLoanService _mediaLoanService;

        public ReportController(IMediaLoanService mediaLoanService)
        {
            _mediaLoanService = mediaLoanService;
        }

        // GET api/report/branch/{branchId}/report?startDate={startDate}&endDate={endDate}
        [HttpGet("branch/{branchId}/report")]
        public async Task<IActionResult> GetBranchReport(int branchId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var borrowingStats = await GetBorrowingStatsAsync(branchId, startDate, endDate);
                var popularMedia = await GetPopularMediaItemsAsync(branchId, startDate, endDate);

                var report = new BranchReportDto
                {
                    BranchId = branchId,
                    StartDate = startDate,
                    EndDate = endDate,
                    BorrowingStats = borrowingStats,
                    PopularItems = popularMedia
                };

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<BorrowingStatsDto> GetBorrowingStatsAsync(int branchId, DateTime startDate, DateTime endDate)
        {
            var loans = await _mediaLoanService.GetLoansByBranchAsync(branchId, startDate, endDate);

            var totalLoans = loans.Count;
            var uniqueUsers = loans.Select(l => l.AccountID).Distinct().Count();
            var overdueLoans = loans.Count(l => l.Status == "Overdue");

            return new BorrowingStatsDto
            {
                TotalLoans = totalLoans,
                UniqueUsers = uniqueUsers,
                OverdueLoans = overdueLoans
            };
        }

        private async Task<IEnumerable<PopularMediaItemDto>> GetPopularMediaItemsAsync(int branchId, DateTime startDate, DateTime endDate)
        {
            var popularMedia = await _mediaLoanService.GetPopularMediaItemsAsync(branchId, startDate, endDate);

            return popularMedia.Select(m => new PopularMediaItemDto
            {
                MediaID = m.MediaID,
                Title = m.Title,
                LoanCount = m.LoanCount
            });
        }
    }
}
