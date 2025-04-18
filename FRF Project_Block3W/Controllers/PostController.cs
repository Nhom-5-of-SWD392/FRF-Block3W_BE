﻿using Data.Models;
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

    [HttpGet("public")]
    public async Task<IActionResult> GetAllApprovedPostsAsync([FromQuery] PostQueryModel query)
    {
        var result = await _postService.GetAllApprovedPostsAsync(query);

        return Ok(result);
    }

    [HttpGet]
	public async Task<IActionResult> GetAllPostByUser([FromQuery]PostQueryModel query)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();

		var result = await _postService.GetAllPostByUser(query,userId);

		return Ok(result);
	}

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostDetail(Guid id)
    {
        var result = await _postService.GetPostDetailAsync(id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostWithMedia([FromBody] PostCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _postService.CreateFullPost(userId, model);

        return Ok(result);
    }

    [HttpPost("{id}/media")]
    public async Task<IActionResult> AddMediaAsync(Guid id, List<IFormFile> file)
    {

        var result = await _postService.AddMediaAsync(id, file);

        return Ok(result);
    }

    [HttpPost("{id}/instruction")]
    public async Task<IActionResult> AddInstructionToPostAsync(Guid id, InstructionRequestModel instructions)
    {
        var result = await _postService.AddInstructionToPostAsync(id, instructions);

        return Ok(result);
    }
}
