using Data.Models;
using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;

[Authorize]
[Route("api/quiz-result")]
[ApiController]
public class QuizResultController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizResultController(IQuizService quizService)
    {
        _quizService = quizService;
    }


    [HttpGet("{id}/quiz-result-details")]
    public async Task<IActionResult> GetQuizResultAsync(Guid id)
    {
        var result = await _quizService.GetQuizResultAsync(id);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMyQuizResultsAsync([FromQuery] QuizResultQueryModel query)
    {
        var userId = User.Claims.GetUserIdFromJwtToken();
        var role = User.Claims.GetUserRoleFromJwtToken();

        var result = await _quizService.GetAllMyQuizResultsAsync(userId, query, role);

        return Ok(result);
    }

    [Authorize("Administrator")]
    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateInterviewAsync([FromBody] EvaluateEssayRequest model)
    {
        var evaluatorId = User.Claims.GetUserIdFromJwtToken();

        var result = await _quizService.EvaluateInterviewAsync(evaluatorId, model);

        return Ok(result);
    }
}
