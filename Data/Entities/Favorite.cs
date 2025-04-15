using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
	public class Favorite : BaseEntities
	{
		//Foreign Keys
		public Guid UserId { get; set; } //Which user create this favorite
		public Guid PostId { get; set; }
		[ForeignKey("UserId")]
		public User? User { get; set; }
		[ForeignKey("PostId")]
		public Post? Post { get; set; }
	}

}
