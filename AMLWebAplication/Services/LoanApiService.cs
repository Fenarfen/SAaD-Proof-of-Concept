using System;
using System.Net.Http;
using System.Threading.Tasks;
using AMLWebAplication.DTOs;
using AMLWebAplication.Data;
using Newtonsoft.Json;

public class LoanApiService
{
    private readonly HttpClient _httpClient;

    public LoanApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MediaLoan>> GetOverdueLoansAsync()
    {
        var response = await _httpClient.GetStringAsync("https://localhost:5001/api/loan/overdue");
        return JsonConvert.DeserializeObject<List<MediaLoan>>(response);
    }

    public async Task<List<MediaLoan>> GetLoansByAccountAsync(int accountId)
    {
        var response = await _httpClient.GetStringAsync($"https://localhost:5001/api/loan/account/{accountId}/loans");
        return JsonConvert.DeserializeObject<List<MediaLoan>>(response);
    }
}