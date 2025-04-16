using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
	public class Notification : BaseEntities
	{
		[Required(ErrorMessage = "Title is required")]
		[StringLength(100, ErrorMessage = "Title can't be longer than 100 characters")]
		public string Title { get; set; } = null!;

		[Required(ErrorMessage = "Content is required")]
		public string Content { get; set; } = null!;

		public bool IsRead { get; set; } = false;

		//Foreign Keys
		public Guid UserId { get; set; } //Which user receive this notification

		[ForeignKey("UserId")]
		public User? User { get; set; }
	}
}
