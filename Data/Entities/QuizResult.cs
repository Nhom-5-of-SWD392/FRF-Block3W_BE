using Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class QuizResult : BaseEntities
{
    public double FinalScore { get; set; }
    [Required(ErrorMessage = "Result is required")]
    public RangeScoreResult Result { get; set; }

    //Foreign Keys
    public Guid QuizMadeById { get; set; }
    [ForeignKey("QuizMadeById")]
    public User? User { get; set; }

    public Guid? EvaluateById { get; set; }
    [ForeignKey("EvaluateById")]
    public User? Evaluator { get; set; }

    public IList<QuizDetail>? QuizDetails { get; set; }
}
