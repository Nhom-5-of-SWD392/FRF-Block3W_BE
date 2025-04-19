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

    [HttpGet("public")]
    public async Task<IActionResult> GetAllApprovedPostsAsync([FromQuery] PostQueryModel query)
    {
        var result = await _postService.GetAllApprovedPostsAsync(query);

        return Ok(result);
    }

	[HttpGet("pending")]
	public async Task<IActionResult> GetAllPendingPostsAsync([FromQuery] PostQueryModel query)
	{
		var result = await _postService.GetAllPendingPostsAsync(query);

		return Ok(result);
	}

	[HttpGet]
	public async Task<IActionResult> GetAllPostByUser([FromQuery]PostQueryModel query)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();
        var role = User.Claims.GetUserRoleFromJwtToken();
		var result = await _postService.GetAllPostByUser(query,userId,role);

		return Ok(result);
	}

	[HttpPatch("{id}/softDelete")]
	public async Task<IActionResult> SoftDelete(Guid id)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();
		var data = await _postService.SoftDelete(userId, id);

		return Ok(data);
	}

	[HttpPatch("{id}/verify")]
	public async Task<IActionResult> VerifyPost(Guid id,bool isConfirm)
	{
        var userId =User.Claims.GetUserIdFromJwtToken();

		var data = await _postService.VerifyPost(isConfirm, id, userId);

		return Ok(data);
	}

	[HttpDelete("id")]
	public async Task<IActionResult> HardDelete(Guid id)
	{
		var userId = User.Claims.GetUserIdFromJwtToken();
		var data = await _postService.HardDelete(userId,id);

		return Ok(data);
	}

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostDetail(Guid id)
    {
        var result = await _postService.GetPostDetailAsync(id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostWithMedia([FromForm] PostCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var userId = User.Claims.GetUserIdFromJwtToken();

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
    public async Task<IActionResult> AddInstructionToPostAsync(Guid id, InstructionRequestModel instructions)
    {
        var result = await _postService.AddInstructionToPostAsync(id, instructions);

        return Ok(result);
    }

    [HttpPost("{id}/ingredients")]
    public async Task<IActionResult> AddIngredientToPostAsync(Guid id, List<IngredientDetailModel> ingredients)
    {
        var result = await _postService.AddIngredientToPostAsync(id, ingredients);

        return Ok(result);
    }
}
