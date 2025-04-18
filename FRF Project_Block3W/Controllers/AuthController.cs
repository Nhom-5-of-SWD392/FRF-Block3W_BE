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
    private readonly IConfiguration _configuration;
    private readonly IGoogleAuthService _googleAuthService;

    public AuthController(IUserService userService, IJwtUtils jwtUtils, IConfiguration configuration, IGoogleAuthService googleAuthService)
    {
        _userService = userService;
        _jwtUtils = jwtUtils;
        _configuration = configuration;
        _googleAuthService = googleAuthService;
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

    [HttpGet("google-login-url")]
    public IActionResult GetGoogleLoginUrl()
    {
        var clientId = _configuration["Authentication:Google:ClientId"];
        var developmentRedirectUri = _configuration["Authentication:Google:DevelopmentRedirectUri"];
        var productionRedirectUri = _configuration["Authentication:Google:ProductionRedirectUri"];

        var scopes = "openid email profile";

        // Sử dụng response_type=code để lấy authorization code
        var loginUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                       $"client_id={clientId}&" +
                       $"redirect_uri={Uri.EscapeDataString(productionRedirectUri!)}&" +
                       $"response_type=code&" +
                       $"scope={Uri.EscapeDataString(scopes)}&" +
                       $"prompt=select_account";

        return Ok(new { url = loginUrl });
    }

    [AllowAnonymous]
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code)
    {
        try
        {
            var developmentRedirectUri = _configuration["Authentication:Google:DevelopmentRedirectUri"];
            var productionRedirectUri = _configuration["Authentication:Google:ProductionRedirectUri"];

            var tokenResponse = await _googleAuthService.ExchangeCodeForTokenAsync(productionRedirectUri!, code);

            var data = await _userService.LoginWithGoogle(tokenResponse.IdToken!);

            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}
