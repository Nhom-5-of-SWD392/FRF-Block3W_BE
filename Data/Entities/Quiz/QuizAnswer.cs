using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities.Quiz
{
	public class QuizAnswer : BaseEntities
	{
		[Required(ErrorMessage = "Answer Text is required")]
		public string AnswerText { get; set; } = null!;
		
		public int Score { get; set; } = 0;

		//Foreign Keys
		public Guid QuizQuestionId { get; set; }
		public QuizQuestion? QuizQuestion { get; set; }
		public Guid QuizId { get; set; }
		public Quiz? Quiz { get; set; }
	}
}
