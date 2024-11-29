using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using OfficeOpenXml.Utils;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AMLWebAplication.Services;

public class APIAuthenticationService() : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Sid, ""),
            new Claim(ClaimTypes.Name, ""),
            new Claim(ClaimTypes.Email, ""),
            new Claim(ClaimTypes.Role, "")
        ]));

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public async void AuthenticateUser(string email, string password)
    {
        string content = JsonSerializer.Serialize(new {email, password });
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://localhost:32772/api/auth/login"),
            Content = new StringContent(content)
        };
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Authorization", "Bearer APIKey");
        request.Headers.Add("User-Agent", "HttpClientFactory-Media");
        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        HttpResponseMessage response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return;
        }

        var responseStream = await response.Content.ReadAsStreamAsync();

        var responseObject = JsonObject.Parse(responseStream);
        string token = responseObject["token"].ToString();
        string name = email.Split('@')[0];
        string role = "Manager";

        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Sid, token),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        ], "Custom Authentication");

        _currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            GetAuthenticationStateAsync());
    }

    public async Task<bool> UserHasRole(string role)
    {
        return (await GetAuthenticationStateAsync()).User.HasClaim(ClaimTypes.Role, role);
    }
}
