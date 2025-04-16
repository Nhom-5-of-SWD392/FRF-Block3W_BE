using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class QuizAnswer : BaseEntities
{
    public string? AnswerText { get; set; }
    public int Score { get; set; } = 0;

    //Foreign Keys
    public Guid QuizQuestionId { get; set; }
    [ForeignKey("QuizQuestionId")]
    public QuizQuestion? QuizQuestion { get; set; }

    public Guid QuizId { get; set; }
    [ForeignKey("QuizId")]
    public Quiz? Quiz { get; set; }

    public IList<QuizDetail>? QuizDetails { get; set; }
}
