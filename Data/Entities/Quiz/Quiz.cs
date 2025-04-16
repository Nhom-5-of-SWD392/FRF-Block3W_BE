using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities.Quiz
{
	public class Quiz : BaseEntities
	{
		[Required(ErrorMessage ="Quiz name is required")]
		public string Name { get; set; } = null!;

		[Required(ErrorMessage = "Quiz description is required")]
		public string Description { get; set; } = null!;

		[Required(ErrorMessage = "Quiz type is required")]
		public string Type { get; set; } = null!;
	}
}
