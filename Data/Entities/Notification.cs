using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class Notification : BaseEntities
{
	[Required(ErrorMessage = "Title is required")]
	[StringLength(100, ErrorMessage = "Title can't be longer than 100 characters")]
	public string Title { get; set; } = string.Empty;
	[Required(ErrorMessage = "Content is required")]
	public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;

	//Foreign Keys
	public Guid UserId { get; set; }
	[ForeignKey("UserId")]
	public User? User { get; set; }
}
