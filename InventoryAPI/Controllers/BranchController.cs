using InventoryAPI.Interfaces;
using InventoryAPI.Models;
using InventoryAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InventoryAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BranchController(
    BranchRepository branchRepository,
    IAuthenticationService authenticationService) : ControllerBase
{
    private const string AUTHLEVEL = "Manager";

    private readonly BranchRepository _branchRepository = branchRepository;
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
    public IActionResult GetBranches()
    {
        Account? account = Authenticate(Request);

        if (account == null)
            return Unauthorized();

        try
        {
            return Ok(_branchRepository.GetBranches());
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
