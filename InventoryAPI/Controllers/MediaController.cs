using InventoryAPI.Models;
using InventoryAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediaController(MediaRepository mediaRepository) : ControllerBase
{
    private readonly MediaRepository _mediaRepository = mediaRepository;

    [HttpGet]
    public List<Media> GetAllMedia()
    {
        List<Media> mediaList = [];

        return mediaList;
    }
}
