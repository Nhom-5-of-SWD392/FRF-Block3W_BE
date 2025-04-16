using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
	public class Post : BaseEntities
	{
		[Required(ErrorMessage = "Title is required")]
		[StringLength(100, ErrorMessage = "Title can't be longer than 100 characters")]
		public string Title { get; set; } = null!;

		[Required(ErrorMessage = "Content is required")]
		public string Content { get; set; } = null!;	

		//Todo: Chưa hiểu rõ về rating lắm, cần check lại
		public int RatingNumber { get; set; } = 0;

		public PostStatus Status { get; set; } = PostStatus.Pending;

		//Foreign Keys
		public Guid UserId { get; set; } //Which user create this post

		[ForeignKey("UserId")]
		public User? User { get; set; } 

	}
}
