using Data.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class Reaction : BaseEntities
{
	public ReactionType ReactionType { get; set; }

	//Foreign Keys
	public Guid UserId { get; set; }
	[ForeignKey("UserId")]
	public User? User { get; set; }

    public Guid CommentId { get; set; }
    [ForeignKey("CommentId")]
    public Comment? Comment { get; set; }
}
