using Data.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class QuizQuestion : BaseEntities
{
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType Type { get; set; }

    //Foreign Keys
    public Guid QuizId { get; set; }
    [ForeignKey("QuizId")]
    public Quiz? Quiz { get; set; }

    public IList<QuizAnswer>? QuizAnswers { get; set; }
    public IList<QuizDetail>? QuizDetails { get; set; }
}
