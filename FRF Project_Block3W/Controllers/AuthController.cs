using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Core;
using Service.Utilities;


namespace FRF_Project_Block3W.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtUtils _jwtUtils;

    public AuthController(IUserService userService, IJwtUtils jwtUtils)
    {
        _userService = userService;
        _jwtUtils = jwtUtils;
    }

    [AllowAnonymous]
    [HttpPost("signin")]
    public async Task<IActionResult> Login(UserRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var data = await _userService.Login(model);
        return Ok(data);
    }

    //[AllowAnonymous]
    //[HttpPost("signin-google")]
    //public async Task<IActionResult> LoginWithGoogle([FromQuery] GoogleLoginRequest request)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return BadRequest();
    //    }
    //    var data = await _userService.LoginWithGoogle(request.GoogleIdToken);

    //    return Ok(data);
    //}
}
