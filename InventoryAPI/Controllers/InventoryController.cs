using InventoryAPI.Interfaces;
using InventoryAPI.Models;
using InventoryAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InventoryAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InventoryController(
    MediaRepository mediaRepository,
    IAuthenticationService authenticationService) : ControllerBase
{
    private readonly MediaRepository _mediaRepository = mediaRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    [HttpGet]
    public IActionResult GetInventory()
    {
        string token = _authenticationService.GetToken(Request);

        if (token.IsNullOrEmpty())
            return Unauthorized();

        User? user = _authenticationService.GetUser(token).Result;

        if (user == null || user.Role.Name != "Manager")
            return Unauthorized();

        try
        {
            return Ok(_mediaRepository.GetMediaByCity(user.Address.City.ID));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
