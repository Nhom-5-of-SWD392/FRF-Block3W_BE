using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
	public class Reaction : BaseEntities
	{
		[Required(ErrorMessage = "Reaction Type is required")]
		public string ReactionType { get; set; } = null!;

		//Foreign Keys
		public Guid UserId { get; set; } //Which user create this reaction

		[ForeignKey("UserId")]
		public User? User { get; set; }

	}
}
