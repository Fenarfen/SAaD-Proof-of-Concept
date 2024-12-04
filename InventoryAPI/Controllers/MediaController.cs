using InventoryAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediaController(MediaRepository mediaRepository) : ControllerBase
{
    private readonly MediaRepository _mediaRepository = mediaRepository;

    [HttpGet]
    public async Task<IActionResult> GetAllMedia()
    {
        try
        {
            return Ok(await _mediaRepository.GetAllMedia(Request.Path));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
