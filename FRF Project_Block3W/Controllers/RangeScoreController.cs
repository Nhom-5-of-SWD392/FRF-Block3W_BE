using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;

[Route("api/quiz-range-score")]
[ApiController]
public class RangeScoreController : ControllerBase
{
    private readonly IRangeScoreService _rangeScoreService;

    public RangeScoreController(IRangeScoreService rangeScoreService)
    {
        _rangeScoreService = rangeScoreService;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRangeScoreAsync(Guid id)
    {
        var result = await _rangeScoreService.DeleteRangeScoreAsync(id);

        return Ok(result);
    }
}
