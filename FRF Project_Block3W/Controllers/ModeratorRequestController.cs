using Data.Models;
using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;

[Route("api/moderator-request")]
[ApiController]
public class ModeratorRequestController : ControllerBase
{
    private readonly IUserService _userService;

    public ModeratorRequestController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRequestAsync([FromQuery] RequestQueryModel model)
    {
        var result = await _userService.GetAllApplicationsAsync(model);
        return Ok(result);
    }

    [HttpPut("{requesterId}/processing-moderator")]
    public async Task<IActionResult> ProcessModeratorApplicationAsync(Guid requesterId, ModeratorApplicationApproveModel model)
    {
        var confirmedId = User.Claims.GetUserIdFromJwtToken();

        await _userService.ProcessModeratorApplicationAsync(confirmedId, requesterId, model);
        return Ok("Approved");
    }
}
