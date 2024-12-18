using InventoryAPI.Interfaces;
using InventoryAPI.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace InventoryAPI.Services;

public class AuthenticationService : IAuthenticationService
{
    public string GetToken(HttpRequest request)
    {
        string? authHeader = request.Headers[HeaderNames.Authorization];

        if (authHeader.IsNullOrEmpty())
            return string.Empty;

        string[] headers = authHeader.Split(" ");

        if (headers.Length != 2)
            return string.Empty;

        return headers[1];
    }

    public async Task<Account?> GetAccount(string token)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"http://host.docker.internal:32777/api/auth/get-city-role-by-token/{token}"),
        };
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Authorization", "Bearer testinventorykey");
        request.Headers.Add("User-Agent", "HttpClientFactory-Media");

        HttpClient client = new();

        HttpResponseMessage response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var jsonResponse = JsonObject.Parse(await response.Content.ReadAsStringAsync());
        string role = jsonResponse["role"].GetValue<string>();
        string city = jsonResponse["city"].GetValue<string>();

        return new()
        {
            ID = 0,
            Token = new()
            {
                ID = 0,
                UserID = 1,
                Value = token,
            },
            Role = new()
            {
                ID = 0,
                Name = role
            },
            Address = new() {
                Id = 0,
                City = city,
            }
        };
    }
}
