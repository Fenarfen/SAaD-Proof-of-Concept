using InventoryAPI.Models;
using InventoryAPI.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediaController(MediaRepository mediaRepository) : ControllerBase
{
    private readonly MediaRepository _mediaRepository = mediaRepository;

    [HttpGet]
    public IActionResult GetAllMedia()
    {
        List<Media> mediaList = [];
        try
        {
            mediaList = _mediaRepository.GetAllMedia();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

        return Ok(mediaList);
    }
}
