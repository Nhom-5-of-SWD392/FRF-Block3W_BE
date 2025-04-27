using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Entities;

public class Comment : BaseEntities
{
	public string Content { get; set; } = string.Empty;
	
	//Foreign Keys
	public Guid? ParentCommentId { get; set; } 

	[ForeignKey("ParentCommentId")]
	public Comment? ParentComment { get; set; } 

	[ForeignKey("PostId")]
    public Guid PostId { get; set; }
    public Post? Post { get; set; }

	[ForeignKey("UserId")]
    public Guid UserId { get; set; }
    public User? User { get; set; }

	public ICollection<Reaction>? Reactions { get; set; }
    public ICollection<Comment>? ChildComments { get; set; }
}
