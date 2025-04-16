﻿using Data.Models;
using FRF_Project_Block3W.Claims;
using Microsoft.AspNetCore.Mvc;
using Service.Core;

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
        var data = await _quizService.GetAll(query);

        return Ok(data);
    }

    [HttpGet("{id}/quiz-details")]
    public async Task<IActionResult> GetQuizDetailAsync(Guid id)
    {
        var data = await _quizService.GetQuizDetailAsync(id);

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuiz(CreateQuizRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = User.Claims.GetUserIdFromJwtToken();

        var data = await _quizService.CreateFullQuizAsync(userId, model);

        return Ok(data);
    }

    [HttpPost("{id}/quiz-range-scores")]
    public async Task<IActionResult> AddQuizRangeScore(Guid id, List<QuizRangeScoreCreateModel> models)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = User.Claims.GetUserIdFromJwtToken();

        var data = await _quizService.AddQuizRangeScore(userId, id, models);

        return Ok(data);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        var data = await _quizService.SoftDelete(id);

        return Ok(data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        var data = await _quizService.HardDelete(id);

        return Ok(data);
    }


}
