using Data.Models;
using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;

[Route("api/reaction")]
[ApiController]
[Authorize]
public class ReactionController : ControllerBase
{
	private readonly IReactionService _reactionService;


	public ReactionController(IReactionService reactionService)
	{
		_reactionService = reactionService;
	}

	/*
	[HttpPost]
	public async Task<IActionResult> CreateReaction([FromBody] ReactionCreateModel model)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		var userId = User.Claims.GetUserIdFromJwtToken();
		var result = await _reactionService.CreateReaction(model, userId);
		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetReactionByCommentId(Guid id)
	{
		var result = await _reactionService.GetReactionByCommentId(id);
		return Ok(result);
	}
	*/
}
