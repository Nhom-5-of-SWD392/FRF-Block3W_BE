using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class QuizDetail : BaseEntities
{
    public double EvaluationScore { get; set; } = 0;

    //Foreign Keys
    public Guid QuizQuestionId { get; set; }
    [ForeignKey("QuizQuestionId")]
    public QuizQuestion? QuizQuestion { get; set; }

    public Guid QuizId { get; set; }
    [ForeignKey("QuizId")]
    public Quiz? Quiz { get; set; }

    public Guid QuizAnswerId { get; set; }
    [ForeignKey("QuizAnswerId")]
    public QuizAnswer? QuizAnswer { get; set; }

    public Guid QuizResultId { get; set; }
    [ForeignKey("QuizResultId")]
    public QuizResult? QuizResult { get; set; }
}
