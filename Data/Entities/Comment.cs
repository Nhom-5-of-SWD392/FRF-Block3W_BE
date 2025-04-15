using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
	public class Comment : BaseEntities
	{
		[Required(ErrorMessage = "Comment Content is required")]
		public string Content { get; set; } = null!;
		
		//Foreign Keys
		public Guid PostId { get; set; }
		public Guid UserId { get; set; } //Which user create this comment

		public Guid? ParentId { get; set; } //Which comment is this comment reply to

		[ForeignKey("ParentId")]
		public Comment? Parent { get; set; } //Which comment is this comment reply to

		[ForeignKey("PostId")]
		public Post? Post { get; set; }
		[ForeignKey("UserId")]
		public User? User { get; set; }
	}
}
