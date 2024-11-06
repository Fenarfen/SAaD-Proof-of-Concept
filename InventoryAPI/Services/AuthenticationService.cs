using InventoryAPI.Interfaces;
using InventoryAPI.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

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

    public async Task<User> GetUser(string token)
    {
        // TODO:
        // Call Authentication API with bearer token to get User information
        // Fake call for now

        User user = new()
        {
            ID = 1,
            Token = new()
            {
                ID = 1,
                UserID = 1,
                Value = token,
                Created = DateTime.Now,
            },
            Address = new()
            {
                ID = 1,
                City =
                {
                    ID = 2,
                    Name = "London",
                },
                FirstLine = "111 Arundle Street",
                PostCode = "S78YJ"
            },
            Role = new()
            {
                ID = 1,
                Name = "Manager"
            },
            Username = "SamSam",
            Password = "ndwa638nd3v7732",
            Email = @"sam@sam.co.uk",
            FirstName = "Sam",
            LastName = "Sam",
            Created = DateTime.Now,
            Verified = true
        };
        
        return user;
    }
}
