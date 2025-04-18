using Data.Models;
using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;


[Route("api/post")]
[ApiController]
public class PostController : ControllerBase
{
	private readonly IPostService _postService;
	public PostController(IPostService postService)
	{
		_postService = postService;
	}

	[HttpPost]
	public async Task<IActionResult> CreatePost([FromBody] PostCreateModel model)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		var userId = User.Claims.GetUserIdFromJwtToken();		

		var result = await _postService.CreateFullPost(userId, model);

		return Ok(result);
	}

	[HttpGet]
	public async Task<IActionResult> GetAllPostByUserId([FromQuery] PostQueryModel query)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();

		var result = await _postService.GetAllPostByUser(query,userId);

		if (result == null)
		{
			return NotFound();
		}

		return Ok(result);
	}

	[HttpPatch("id")]
	public async Task<IActionResult> SoftDelete(Guid id)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();
		var data = await _postService.SoftDelete(userId, id);

		return Ok(data);
	}

	[HttpDelete("id")]
	public async Task<IActionResult> HardDelete(Guid id)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();
		var data = await _postService.HardDelete(userId,id);

		return Ok(data);
	}
}
