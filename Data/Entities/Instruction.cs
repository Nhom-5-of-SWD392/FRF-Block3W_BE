using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class Instruction : BaseEntities
{
    public string? ImageUrl { get; set; }
    public string Content { get; set; } = string.Empty;

    //Foreign Keys
    public Guid PostId { get; set; }
    [ForeignKey("PostId")]
    public Post? Post { get; set; }
}
