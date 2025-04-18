using Microsoft.AspNetCore.Http;

namespace Data.Models;

public class InstructionDetailModel
{
    public IFormFile? Image { get; set; }
    public string? Content { get; set; }
}

public class InstructionRequestModel
{
    public string Content { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
}

public class InstructionCreateModel
{
    public string? ImageUrl { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }
}