using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Mvc;
using Service.Core;
using Data.Models;

namespace FRF_Project_Block3W.Controllers;

[Route("api/quiz")]
[ApiController]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QuizQueryModel query)
    {
        var result = await _quizService.GetAll(query);

        return Ok(result);
    }

    [HttpGet("{id}/quiz-details")]
    public async Task<IActionResult> GetQuizDetailAsync(Guid id)
    {
        var result = await _quizService.GetQuizDetailAsync(id);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuiz(CreateQuizRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _quizService.CreateFullQuizAsync(userId, model);

        return Ok(result);
    }

    [HttpPost("{id}/quiz-range-scores")]
    public async Task<IActionResult> AddQuizRangeScore(Guid id, List<QuizRangeScoreCreateModel> models)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _quizService.AddQuizRangeScore(userId, id, models);

        return Ok(result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        var result = await _quizService.SoftDelete(id);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        var result = await _quizService.HardDelete(id);

        return Ok(result);
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = User.Claims.GetUserIdFromJwtToken();

        var result = await _quizService.SubmitQuizAsync(userId, request);

        return Ok(result);
    }
}
