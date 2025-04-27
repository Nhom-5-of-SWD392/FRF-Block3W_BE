using CloudinaryDotNet.Actions;
using Data.Models;
using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Core;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FRF_Project_Block3W.Controllers;


[Route("api/post")]
[ApiController]
[Authorize]
public class PostController : ControllerBase
{
	private readonly IPostService _postService;
    

	public PostController(IPostService postService)
	{
		_postService = postService;
       
	}

    [AllowAnonymous]
    [HttpGet("public")]
    public async Task<IActionResult> GetAllApprovedPostsAsync([FromQuery] PostApproveQueryModel query)
    {
        var result = await _postService.GetAllApprovedPostsAsync(query);

        return Ok(result);
    }

	[HttpGet("own-post")]
	public async Task<IActionResult> GetAllPostByUser([FromQuery]PostQueryModel query)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();

        var role = User.Claims.GetUserRoleFromJwtToken();

		var result = await _postService.GetAllPostByUser(query,userId,role);

		return Ok(result);
	}

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] PostUpdateModel model)
    {
        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _postService.UpdatePostAsync(userId, id, model);

        return Ok(result);
    }


    [HttpPatch("{id}/soft-delete")]
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

    [HttpGet("{id}/post-details")]
    public async Task<IActionResult> GetPostDetail(Guid id)
    {
        var result = await _postService.GetPostDetailAsync(id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostWithMedia(
        [FromForm] string title,
        [FromForm] string content,
        [FromForm] List<Guid> topics,
        [FromForm] List<IFormFile> medias)
    {
        var userId = User.Claims.GetUserIdFromJwtToken();

        var model = new PostInputModel
        {
            Title = title,
            Content = content,
            Topics = topics ?? new(),
            Medias = medias
        };

        var result = await _postService.CreateFullPost(userId, model);

        return Ok(result);
    }


    [HttpPost("{id}/medias")]
    public async Task<IActionResult> AddMediaAsync(Guid id, List<IFormFile> file)
    {

        var result = await _postService.AddMediaAsync(id, file);

        return Ok(result);
    }

    [HttpPost("{id}/instruction")]
    public async Task<IActionResult> AddInstructionToPostAsync(Guid id, [FromForm]InstructionRequestModel instructions)
    {
        var result = await _postService.AddInstructionToPostAsync(id, instructions);

        return Ok(result);
    }

    [HttpPut("{id}/instructions")]
    public async Task<IActionResult> UpdateInstructionsFromPostAsync(Guid id, [FromForm] InstructionUpdateModel instructions)
    {
        var userId = User.Claims.GetUserIdFromJwtToken();

        await _postService.UpdateInstructionFromPostAsync(userId, id, instructions);

        return NoContent();
    }


    [HttpPost("{id}/ingredients")]
    public async Task<IActionResult> AddIngredientToPostAsync(Guid id, List<IngredientDetailModel> ingredients)
    {
        var result = await _postService.AddIngredientToPostAsync(id, ingredients);

        return Ok(result);
    }

	[HttpPost("{id}/topics")]
	public async Task<IActionResult> AddTopicsToPostAsync(Guid id, [FromBody] List<Guid> topicIds)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();
		await _postService.AddTopicsToPostAsync(id, userId, topicIds);
		return NoContent();
	}

	[HttpPost("{id}/leave-comment")]
    [Authorize(Roles = "Member, Administrator")]
    public async Task<IActionResult> LeaveComment(Guid id, [FromBody] CommentCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _postService.AddCommentAsync(userId, id, model);

        return Ok(result);
    }

    [HttpGet("{id}/comments")]
    public async Task<IActionResult> GetPostComments(Guid id)
    {
        var result = await _postService.GetCommentsByPostIdAsync(id);

        return Ok(result);
    }

    [HttpPut("{id}/confirm-post")]
    public async Task<IActionResult> ApproveOrRejectPostAsync(Guid id, ConfirmPost model)
    {
        var userId = User.Claims.GetUserIdFromJwtToken();

        var result =  await _postService.ApproveOrRejectPostAsync(userId, id, model);

        return Ok(result);
    }

    [HttpPost("{id}/favorite")]
    public async Task<IActionResult> AddPostToFavoriteList(Guid id)
    {
        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _postService.AddPostToFavoriteList(id, userId);

        return Ok(result);
    }

    [HttpDelete("{id}/favorite")]
    public async Task<IActionResult> RemovePostFromFavoriteList(Guid id)
    {
        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _postService.RemovePostFromFavoriteList(id, userId);

        return Ok(result);
    }

	//chức năng update nguyên liệu cho post, Update 1 hoặc nhiều nguyên liệu đã có trong post
	[HttpPut("{id}/ingredients")]
	public async Task<IActionResult> UpdateIngredientsAsync(Guid id, [FromBody] List<IngredientUpdateModel> ingredients)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();

		await _postService.UpdateIngredientsAsync(id, userId, ingredients);

		return NoContent();
	}

    [HttpDelete("{id}/topics")]
	public async Task<IActionResult> RemoveTopicsFromPostAsync(Guid id, [FromBody] List<Guid> topicIds)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();
		await _postService.RemoveTopicsFromPostAsync(id, userId, topicIds);
		return NoContent();
	}

	[HttpDelete("{id}/Instruction")]
	public async Task<IActionResult> DeleteInstructionAsync(Guid id, [FromQuery] Guid instructionId)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();
		await _postService.DeleteInstructionAsync(userId, id, instructionId);
		return NoContent();
	}

	[HttpDelete("{id}/ingredients")]
    public async Task<IActionResult> DeleteIngredientAsync(Guid id, [FromQuery] Guid ingredientId)
    {
        var userId = User.Claims.GetUserIdFromJwtToken();
        await _postService.DeleteIngredientAsync(userId, id, ingredientId);
        return NoContent();
    }
}
