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

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveMedia(Guid id)
    {
        var result = await _mediaService.RemoveMediaAsync(id);

        return Ok(result);
    }
}
