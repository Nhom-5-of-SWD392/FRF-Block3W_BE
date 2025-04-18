using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;

[Route("api/media")]
[ApiController]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    //[HttpPost("{postId}")]
    //public async Task<IActionResult> AddMedia(Guid postId, [FromForm] IFormFile file)
    //{
    //    var url = await _mediaService.AddMediaAsync(postId, file);

    //    return Ok(url);
    //}

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> RemoveMedia(Guid id)
    //{
    //    var result = await _mediaService.RemoveMediaAsync(id);

    //    return Ok(result);
    //}
}
