using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReportAPI.Services;
using ReportAPI.DTOs;
using ReportAPI.Models;

// endpoints return: / api/loan/overdue & /api/loan/account/1/loans
[Route("api/[controller]")]
[ApiController]
public class LoanController : ControllerBase
{
    private readonly IMediaLoanService _mediaLoanService;

    public LoanController(IMediaLoanService mediaLoanService)
    {
        _mediaLoanService = mediaLoanService;
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdueLoans()
    {
        try
        {
            var overdueLoans = await _mediaLoanService.GetOverdueLoansAsync();

            if (overdueLoans == null || !overdueLoans.Any())
                return NotFound(new { message = "No overdue loans found." });

            return Ok(overdueLoans);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("account/{accountId}/loans")]
    public async Task<IActionResult> GetLoansByAccount(int accountId)
    {
        try
        {
            var loans = await _mediaLoanService.GetLoansByAccountAsync(accountId);

            if (loans == null || !loans.Any())
                return NotFound(new { message = "No loans found for this account." });

            return Ok(loans);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

