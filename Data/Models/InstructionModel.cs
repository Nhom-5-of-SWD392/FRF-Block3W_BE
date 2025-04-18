using Microsoft.AspNetCore.Http;

namespace Data.Models;

public class InstructionDetailModel
{
    public IFormFile? Image { get; set; }
    public string? Content { get; set; }
}
