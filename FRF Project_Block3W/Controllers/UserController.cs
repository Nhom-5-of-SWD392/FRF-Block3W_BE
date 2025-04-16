using Data.Models;
using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterUserModel model)
    {
        var result = await _userService.RegisterAsync(model);
        return Ok(result);
    }

    [HttpPost("register-moderator")]
    public async Task<IActionResult> RegisterModerator()
    {
        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _userService.RegisterModeratorAsync(userId);
        return Ok(result);
    }

    [HttpPut("approve-moderator/{requesterId}")]
    public async Task<IActionResult> ProcessModeratorApplicationAsync(Guid requesterId, ModeratorApplicationApproveModel model)
    {
        var confirmedId = User.Claims.GetUserIdFromJwtToken();

        await _userService.ProcessModeratorApplicationAsync(confirmedId, requesterId, model);
        return Ok("Approved");
    }


    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestModel model)
    {
        var resetLink = await _userService.RequestPasswordResetAsync(model);
        return Ok(new { Message = "Reset password email sent.", ResetLink = resetLink });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetModel model)
    {
        var result = await _userService.ResetPasswordAsync(model);
        return Ok(result);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _userService.ChangePasswordAsync(userId, model);
        return Ok(result);
    }
}
