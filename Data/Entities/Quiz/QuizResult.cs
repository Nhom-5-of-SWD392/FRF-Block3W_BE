using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities.Quiz
{
	public class QuizResult : BaseEntities
	{
		
		public int FinalScore { get; set; } = 0;
		[Required(ErrorMessage = "Result is required")]
		public string Result { get; set; } = null!;

		//Foreign Keys
		public Guid QuizDetailId{ get; set; }
		public QuizDetail? QuizDetail { get; set; }

		public Guid UserId { get; set; } 
		public User? User { get; set; } //Which user take the quiz

		public Guid EvalulatorId { get; set; }
		public User? Evaluator { get; set; } //Who evaluate the quiz
	}
}
