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

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] UserQueryModel model)
    {
        var result = await _userService.GetAll(model);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetById(id);

        return Ok(result);
    }

    [HttpPut("update-personal-profile")]
    public async Task<IActionResult> UpdatePersonalInformation([FromBody] UserUpdateModel model)
    {
        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _userService.UpdatePersonalInformation(userId, model);

        return Ok(result);
    }

    [HttpPut("update-avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is null or empty");
        }
        if (Path.GetExtension(file.FileName) != ".png" && Path.GetExtension(file.FileName) != ".jpeg" && Path.GetExtension(file.FileName) != ".jpg")
        {
            return BadRequest("Only Image file are allowed");
        }

        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _userService.UpdateAvatarImage(userId, file);

        return Ok(result);
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterUserModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

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
