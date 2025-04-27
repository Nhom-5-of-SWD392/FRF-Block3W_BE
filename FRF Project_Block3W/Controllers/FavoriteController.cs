using Data.Models;
using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;

[Route("api/favorite")]
[ApiController]
[Authorize]
public class FavoriteController : ControllerBase
{
	private readonly IFavoriteService _favoriteService;
	public FavoriteController(IFavoriteService favoriteService)
	{
		_favoriteService = favoriteService;
	}

	[HttpGet]
	public async Task<IActionResult> GetFavoriteListByUser([FromQuery] FavoriteQueryModel query)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();

		var data = await _favoriteService.GetFavoriteListByUser(query, userId);

		return Ok(data);
	}
}
