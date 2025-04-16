using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities.Quiz
{
	public class QuizRangeScore : BaseEntities
	{
		public int MinScore { get; set; } = 0;
		public int MaxScore { get; set; } = 0;

		[Required(ErrorMessage = "Result is required")]
		public string Result { get; set; } = null!;

		//Foreign Keys
		public Guid QuizId { get; set; }
		public Quiz? Quiz { get; set; }
	}
}
