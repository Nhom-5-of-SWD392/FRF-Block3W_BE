using Data.Models;
using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;

[Route("api/favorite")]
[ApiController]
public class FavoriteController : ControllerBase
{
	private readonly IFavoriteService _favoriteService;
	public FavoriteController(IFavoriteService favoriteService)
	{
		_favoriteService = favoriteService;
	}

	[HttpGet]
	public async Task<IActionResult> GetByUser([FromQuery] FavoriteQueryModel query)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();
		var role = User.Claims.GetUserRoleFromJwtToken();
		var data = await _favoriteService.GetFavoriteListByUser(query,userId,role);
		return Ok(data);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] FavoriteCreateModel model)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}
		var userId = User.Claims.GetUserIdFromJwtToken();
		var data = await _favoriteService.CreateFavorite(model, userId);
		return Ok(data);
	}


}
