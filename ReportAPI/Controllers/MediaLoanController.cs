using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReportAPI.Services;
using ReportAPI.DTOs;
using ReportAPI.Models;


[ApiController]
[Route("api/[controller]")]
public class MediaLoanController : ControllerBase
{
    private readonly IMediaLoanService _mediaLoanService;

    public MediaLoanController(IMediaLoanService mediaLoanService)
    {
        _mediaLoanService = mediaLoanService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MediaLoan>>> GetMediaLoans()
    {
        try
        {
            var loans = await _mediaLoanService.GetMediaLoansAsync();
            return Ok(loans);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}