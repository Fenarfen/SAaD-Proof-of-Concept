using System;
using System.Net.Http;
using System.Threading.Tasks;
using AMLWebAplication.DTOs;
using Newtonsoft.Json;

public class ReportApiService
{
    private readonly HttpClient _httpClient;

    public ReportApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BranchReportDto> GetBranchReportAsync(int branchId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var url = $"https://localhost:5001/api/report/branch/{branchId}/report?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            var response = await _httpClient.GetStringAsync(url);
            var branchReport = JsonConvert.DeserializeObject<BranchReportDto>(response);
            return branchReport;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching report: {ex.Message}");
            return null;
        }
    }
}
