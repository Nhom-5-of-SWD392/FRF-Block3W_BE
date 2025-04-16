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
