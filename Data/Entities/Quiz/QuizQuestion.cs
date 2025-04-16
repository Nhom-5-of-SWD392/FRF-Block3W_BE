using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities.Quiz
{
	public class QuizQuestion : BaseEntities
	{
		[Required(ErrorMessage = "Question Text is required")]
		public string QuestionText { get; set; } = null!;
		[Required(ErrorMessage = "Question Type is required")]
		public string QuestionType { get; set; } = null!;

		//Foreign Keys
		public Guid QuizId { get; set; }
		public Quiz? Quiz { get; set; }
	}
}
