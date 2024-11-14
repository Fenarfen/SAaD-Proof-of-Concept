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
    private const string AUTHLEVEL = "Manager";

    private readonly MediaRepository _mediaRepository = mediaRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    private Account? Authenticate(HttpRequest request)
    {
        string token = _authenticationService.GetToken(request);

        if (token.IsNullOrEmpty())
            return null;

        Account? account = _authenticationService.GetAccount(token).Result;

        if (account == null || account.Role.Name != AUTHLEVEL)
            return null;

        return account;
    }

    [HttpGet]
    public IActionResult GetInventory()
    {
        Account? account = Authenticate(Request);

        if (account == null)
            return Unauthorized();

        try
        {
            return Ok(_mediaRepository.GetMediaByCity("Sheffield"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("Transfer")]
    public IActionResult GetTransfers()
    {
        Account? account = Authenticate(Request);

        if (account == null)
            return Unauthorized();

        try
        {
            return Ok(_mediaRepository.GetTransfers(account.ID));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Route("Transfer")]
    public IActionResult CreateTransfer(List<MediaTransfer> transfers)
    {
        Account? account = Authenticate(Request);

        if (account == null)
            return Unauthorized();

        if (transfers.IsNullOrEmpty())
            return BadRequest("No transfer(s) supplied");

        try
        {
            foreach(var transfer in transfers)
            {
                int createdID = _mediaRepository.CreateTransfer(transfer);
                
                if (createdID == 0)
                    return StatusCode(500, "Transfer could not be created");
                
                transfer.ID = createdID;
            }

            return Ok(transfers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
