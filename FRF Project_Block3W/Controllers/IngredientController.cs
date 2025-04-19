using Microsoft.AspNetCore.Mvc;
using Service.Core;

namespace FRF_Project_Block3W.Controllers;

[Route("api/ingredient")]
[ApiController]
public class IngredientController : ControllerBase
{
    private readonly IIngredientService _ingredientService;

    public IngredientController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngredientAsync(Guid id)
    {
        var result = await _ingredientService.DeleteIngredientAsync(id);

        return Ok(result);
    }
}
