using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;

[Route("api/instruction")]
[ApiController]
public class InstructionController : ControllerBase
{
    private readonly IInstructionService _instructionService;

    public InstructionController(IInstructionService instructionService)
    {
        _instructionService = instructionService;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInstructionAsync(Guid id)
    {
        var result = await _instructionService.DeleteInstructionAsync(id);

        return Ok(result);
    }
}
