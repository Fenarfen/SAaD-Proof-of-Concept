using Microsoft.AspNetCore.Mvc;
using AMLMonitor.Services.Interfaces;

namespace AMLMonitor.Controllers
{
  [Route("api/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("media-usage")]
        public async Task<IActionResult> GetMediaUsageReport(int branchId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var report = await _reportService.GetMediaUsageReport(branchId, startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error generating report.", Details = ex.Message });
            }
        }
    }  
}