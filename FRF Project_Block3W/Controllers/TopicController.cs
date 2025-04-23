using Data.Models;
using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;

[Route("api/topic")]
[ApiController]
[Authorize]
public class TopicController : ControllerBase
{
	private readonly ITopicService _topicService;

	public TopicController(ITopicService topicService)
	{
		_topicService = topicService;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll([FromQuery] TopicQueryModel query)
	{
		var data = await _topicService.GetAll(query);

		return Ok(data);
	}

	[HttpPost]
	public async Task<IActionResult> CreateTopic([FromBody] TopicCreateModel model)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}

		var userId = User.Claims.GetUserIdFromJwtToken();

		var data = await _topicService.Create(userId,model);

		return Ok(data);
	}

    [HttpGet("{id}/posts")]
    public async Task<IActionResult> GetPostsByTopicAsync(Guid id)
    {
        var data = await _topicService.GetPostsByTopicAsync(id);

        return Ok(data);
    }

    [HttpPut("id")]
	public async Task<IActionResult> UpdateTopic(Guid id, [FromBody] TopicUpdateModel model)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}

		var userId = User.Claims.GetUserIdFromJwtToken();  

		var data = await _topicService.Update(userId, id, model);

		return Ok(data);
	}

	[HttpDelete("id")]
	public async Task<IActionResult> Delete(Guid id)
	{
		var data = await _topicService.Delete(id);

		return Ok(data);
	}
}
