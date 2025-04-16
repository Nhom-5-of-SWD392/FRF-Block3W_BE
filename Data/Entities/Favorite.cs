using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class Favorite : BaseEntities
{
	//Foreign Keys
	public Guid UserId { get; set; }
	[ForeignKey("UserId")]
	public User? User { get; set; }

    public Guid PostId { get; set; }
    [ForeignKey("PostId")]
	public Post? Post { get; set; }
}
